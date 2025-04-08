using System.Linq.Expressions;
using Application.Contracts.Grpc;
using Application.Contracts.MessagingContracts;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TagsDto;
using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Commands.TaskCommands.CreateTask;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using TasksService.Domain.Enums;
using TasksService.Domain.Models;

namespace Tests.TasksService.Tasks.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CustomTask>> _validatorMock;
    private readonly Mock<ITaskCreatedProducer> _taskCreatedProducerMock;
    private readonly Mock<IUserGrpcService> _userGrpcMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CustomTask>>();
        _taskCreatedProducerMock = new Mock<ITaskCreatedProducer>();
        _userGrpcMock = new Mock<IUserGrpcService>();

        _handler = new CreateTaskCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _taskCreatedProducerMock.Object,
            _userGrpcMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateTask_WhenDataIsValid()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            TaskId = Guid.NewGuid(),
            UserId = "user123",
            Title = "Test Task",
            Description = "Test Description",
            Category = Category.Work,
            Priority = Priority.High,
            Deadline = DateTime.UtcNow.AddDays(1),
            MinutesBeforeDeadline = 1,
            TaskTags = new List<TagDto>
            {
                new() { TagName = "Sport" }
            }
        };

        var command = new CreateTaskCommand { TaskDto = taskDto };

        var taskEntity = new CustomTask
        {
            Id = Guid.NewGuid(),
            Title = taskDto.Title,
            Description = taskDto.Description,
            Category = taskDto.Category,
            Priority = taskDto.Priority,
            Deadline = taskDto.Deadline,
            MinutesBeforeDeadline = taskDto.MinutesBeforeDeadline,
            TaskTags = new List<Tag> { new() { Name = "Sport" } }
        };

        _mapperMock.Setup(m => m.Map<CustomTask>(command)).Returns(taskEntity);
        _validatorMock.Setup(v => v.ValidateAsync(taskEntity, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new ValidationResult());

        _repositoryMock
            .Setup(r => r.Tag.FindByCondition(It.IsAny<Expression<Func<Tag, bool>>>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskEntity.TaskTags);

        _repositoryMock.Setup(r => r.Task.Create(taskEntity, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

        _userGrpcMock.Setup(u => u.GetUserEmailAsync(taskDto.UserId))
                     .ReturnsAsync("user@example.com");

        _mapperMock.Setup(m => m.Map<CreateTaskEventDto>(taskDto))
                   .Returns(new CreateTaskEventDto());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _validatorMock.Verify(v => v.ValidateAsync(It.IsAny<CustomTask>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Task.Create(It.IsAny<CustomTask>(), It.IsAny<CancellationToken>()), Times.Once);
        _taskCreatedProducerMock.Verify(p => p.PublishTaskCreatedEvent(It.Is<CreateTaskEventDto>(dto =>
            dto.UserEmail == "user@example.com" && dto.TaskId == taskEntity.Id
        )), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}