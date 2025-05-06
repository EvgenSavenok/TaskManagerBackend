using System.Linq.Expressions;
using Application.Contracts.Grpc;
using Application.Contracts.MessagingContracts;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Commands.TaskCommands.UpdateTask;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace ModuleTests.TasksService.Tasks.Commands;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<CustomTask>> _validatorMock = new();
    private readonly Mock<ITaskUpdatedProducer> _producerMock = new();
    private readonly Mock<IUserGrpcService> _grpcMock = new();

    private readonly UpdateTaskCommandHandler _handler;

    public UpdateTaskCommandHandlerTests()
    {
        _handler = new UpdateTaskCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _producerMock.Object,
            _grpcMock.Object
        );
    }
    
    [Fact]
    public async Task Handle_ShouldUpdateTask_WhenValid()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var taskDto = new TaskDto
        {
            TaskId = taskId,
            UserId = "user1",
            Title = "Test",
            TaskTags = [new() { TagName = "tag1" }]
        };

        var existingTask = new CustomTask { Id = taskId };

        _repositoryMock.Setup(r => r.Task.GetTaskByIdAsync(taskId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _mapperMock.Setup(m => m.Map(taskDto, existingTask));

        _validatorMock.Setup(v => v.ValidateAsync(existingTask, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repositoryMock.Setup(r => r.Tag.FindByCondition(It.IsAny<Expression<Func<Tag, bool>>>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag> { new() { Name = "tag1" } });

        _repositoryMock.Setup(r => r.Task.Update(existingTask, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _grpcMock.Setup(g => g.GetUserEmailAsync("user1"))
            .ReturnsAsync("user@mail.com");

        _mapperMock.Setup(m => m.Map<UpdateTaskEventDto>(taskDto))
            .Returns(new UpdateTaskEventDto());

        // Act
        var result = await _handler.Handle(new UpdateTaskCommand { TaskDto = taskDto }, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.Task.Update(existingTask, It.IsAny<CancellationToken>()), Times.Once);
        _producerMock.Verify(p => p.PublishTaskUpdatedEvent(It.IsAny<UpdateTaskEventDto>()), Times.Once);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskDto = new TaskDto { TaskId = Guid.NewGuid() };

        _repositoryMock.Setup(r => r.Task.GetTaskByIdAsync(taskDto.TaskId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomTask)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new UpdateTaskCommand { TaskDto = taskDto }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var taskDto = new TaskDto { TaskId = Guid.NewGuid() };
        var existingTask = new CustomTask();

        _repositoryMock.Setup(r => r.Task.GetTaskByIdAsync(taskDto.TaskId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _mapperMock.Setup(m => m.Map(taskDto, existingTask));

        _validatorMock.Setup(v => v.ValidateAsync(existingTask, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("Title", "Required") }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(new UpdateTaskCommand { TaskDto = taskDto }, CancellationToken.None));
    }
}