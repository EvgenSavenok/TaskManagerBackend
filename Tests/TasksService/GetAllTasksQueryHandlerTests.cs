using System.Linq.Expressions;
using System.Security.Claims;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Queries.TaskQueries.GetAllTasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using TasksService.Domain.Enums;
using TasksService.Domain.Models;

namespace Tests.TasksService;

public class GetAllTasksQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRedisCacheService> _cacheMock;
    private readonly GetAllTasksQueryHandler _handlerTests;

    public GetAllTasksQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _mapperMock = new Mock<IMapper>();
        _cacheMock = new Mock<IRedisCacheService>();

        _handlerTests = new GetAllTasksQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _cacheMock.Object
        );
    }
    
    [Fact]
    public async Task Handle_ReturnsCachedTasks_WhenTasksExistInCache()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cachedTasks = new List<TaskDto>
        {
            new()
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
            },
            new()
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
            }
        };

        _cacheMock.Setup(c => c.GetAsync<IEnumerable<TaskDto>>($"tasks:user:{userId}"))
            .ReturnsAsync(cachedTasks);

        var query = new GetAllTasksQuery { UserId = new Claim("sub", userId.ToString()) };

        // Act
        var result = await _handlerTests.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cachedTasks);
        _repositoryMock.Verify(r => r.Task.FindByCondition(
            It.IsAny<Expression<Func<CustomTask, bool>>>(), 
            It.IsAny<bool>(), 
            It.IsAny<CancellationToken>(), 
            It.IsAny<Expression<Func<CustomTask, object>>>()), 
            Times.Never);
    }
    
    [Fact]
    public async Task Handle_ReturnsTasksFromRepository_WhenCacheIsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tasks = new List<CustomTask>
        {
            new()
            {
                UserId = userId,
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
            },
            new()
            {
                UserId = userId,
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
            }
        };

        var taskDtos = new List<TaskDto>
        {
            new()
            {
                UserId = userId.ToString(),
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
            },
            new()
            {
                UserId = userId.ToString(),
                Title = "Title",
                Description = "Description",
                Category = Category.Work,
                Priority = Priority.High,
                Deadline = DateTime.UtcNow.AddDays(5),
                TaskTags =
                {
                    new() { TagName = "Urgent" },
                    new() { TagName = "Work" }
                },
                TaskComments =
                {
                    new() { Content = "First comment" },
                    new() { Content = "Second comment" }
                }
            }
        };

        _cacheMock.Setup(c => c.GetAsync<IEnumerable<TaskDto>>(
                $"tasks:user:{userId}"))
            .ReturnsAsync((IEnumerable<TaskDto>)null);

        _repositoryMock.Setup(r => r.Task.FindByCondition(
                It.IsAny<Expression<Func<CustomTask, bool>>>(),
                false, 
                It.IsAny<CancellationToken>(), 
                It.IsAny<Expression<Func<CustomTask, object>>[]>()))
            .ReturnsAsync(tasks);

        _mapperMock.Setup(m => m.Map<IEnumerable<TaskDto>>(tasks)).Returns(taskDtos);
        
        var query = new GetAllTasksQuery { UserId = new Claim("sub", userId.ToString()) };
        
        // Act
        var result = await _handlerTests.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(taskDtos);
        
        _cacheMock.Verify(c => c.SetAsync(
            $"tasks:user:{userId}",
            taskDtos, 
            TimeSpan.FromMinutes(10)),
            Times.Never);
    }
}