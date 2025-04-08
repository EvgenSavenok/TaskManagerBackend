using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Queries.TaskQueries.GetTaskById;
using AutoMapper;
using FluentAssertions;
using Moq;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Enums;
using TasksService.Domain.Models;

namespace Tests.TasksService.Tasks.Queries;

public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRedisCacheService> _cacheMock;
    private readonly GetTaskByIdQueryHandler _handlerTests;

    public GetTaskByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _mapperMock = new Mock<IMapper>();
        _cacheMock = new Mock<IRedisCacheService>();

        _handlerTests = new GetTaskByIdQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _cacheMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnsCachedTask_WhenTaskExistsInCache()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var cachedTaskDto = new TaskDto
        {
            UserId = "",
            Title = "Title",
            Description = "Description",
            Category = Category.Work,
            Priority = Priority.High,
            Deadline = DateTime.UtcNow.AddDays(5),
            TaskTags =
            [
                new() { TagName = "Urgent" },
                new() { TagName = "Work" }
            ],
            TaskComments =
            [
                new() { Content = "First comment" },
                new() { Content = "Second comment" }
            ]
        };

        _cacheMock.Setup(c => c.GetAsync<TaskDto>($"task:{taskId}"))
            .ReturnsAsync(cachedTaskDto);

        var query = new GetTaskByIdQuery(taskId);

        // Act
        var result = await _handlerTests.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cachedTaskDto);
        _repositoryMock.Verify(
            r => r.Task.GetTaskByIdAsync(It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact]
    public async Task Handle_ReturnsTaskFromRepository_WhenTaskNotInCache()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new CustomTask
        {
            UserId = Guid.Empty,
            Title = "Title",
            Description = "Description",
            Category = Category.Work,
            Priority = Priority.High,
            Deadline = DateTime.UtcNow.AddDays(5),
            TaskTags =
            [
                new() { Name = "Urgent" },
                new() { Name = "Work" }
            ],
            TaskComments =
            [
                new() { Content = "First comment" },
                new() { Content = "Second comment" }
            ]
        };
        var taskDto = new TaskDto
        {
            UserId = "",
            Title = "Title",
            Description = "Description",
            Category = Category.Work,
            Priority = Priority.High,
            Deadline = DateTime.UtcNow.AddDays(5),
            TaskTags =
            [
                new() { TagName = "Urgent" },
                new() { TagName = "Work" }
            ],
            TaskComments =
            [
                new() { Content = "First comment" },
                new() { Content = "Second comment" }
            ]
        };

        _cacheMock.Setup(c => c.GetAsync<TaskDto>($"task:{taskId}"))
            .ReturnsAsync((TaskDto)null);

        _repositoryMock.Setup(r => r.Task.GetTaskByIdAsync(taskId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _mapperMock.Setup(m => m.Map<TaskDto>(task)).Returns(taskDto);

        var query = new GetTaskByIdQuery(taskId);

        // Act
        var result = await _handlerTests.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(taskDto);
        _cacheMock.Verify(c => c.SetAsync($"task:{taskId}", taskDto, TimeSpan.FromMinutes(10)), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _cacheMock.Setup(c => c.GetAsync<TaskDto>($"task:{taskId}"))
            .ReturnsAsync((TaskDto)null);

        _repositoryMock.Setup(r => r.Task.GetTaskByIdAsync(taskId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomTask)null);

        var query = new GetTaskByIdQuery(taskId);

        // Act & Assert
        await _handlerTests.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}