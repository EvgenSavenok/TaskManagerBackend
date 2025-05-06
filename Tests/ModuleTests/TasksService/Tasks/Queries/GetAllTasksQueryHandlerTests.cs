using System.Security.Claims;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Queries.TaskQueries.GetAllTasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using TasksService.Domain.Enums;
using TasksService.Domain.Models;

namespace ModuleTests.TasksService.Tasks.Queries;

public class GetAllTasksQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTasksQueryHandler _handlerTests;

    public GetAllTasksQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryManager>();
        _mapperMock = new Mock<IMapper>();

        _handlerTests = new GetAllTasksQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object
        );
    }
    
    [Fact]
    public async Task Handle_ReturnsCachedTasks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

        var query = new GetAllTasksQuery { UserId = userClaim };

        var expectedTasks = new List<CustomTask>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "Title",
                Description = "Description",
                Category = Category.Work,
                Priority = Priority.High,
                Deadline = DateTime.UtcNow.AddDays(5),
                TaskTags = new List<Tag>
                {
                    new() { Name = "Study" },
                    new() { Name = "Work" }
                },
                TaskComments = new List<Comment>
                {
                    new() { Content = "First comment" },
                    new() { Content = "Second comment" }
                }
            }
        };

        var expectedDtos = new List<TaskDto>
        {
            new()
            {
                UserId = userId.ToString(),
                Title = "Title",
                Description = "Description",
                Category = Category.Work,
                Priority = Priority.High,
                Deadline = expectedTasks[0].Deadline,
                TaskTags =
                [
                    new() { TagName = "Study" },
                    new() { TagName = "Work" }
                ],
                TaskComments =
                [
                    new() { Content = "First comment" },
                    new() { Content = "Second comment" }
                ]
            }
        };

        _repositoryMock.Setup(r => r.Task.GetAllTasks(false, It.IsAny<CancellationToken>(), userId))
            .ReturnsAsync(expectedTasks);

        _mapperMock.Setup(m => m.Map<IEnumerable<TaskDto>>(expectedTasks))
            .Returns(expectedDtos);

        // Act
        var result = await _handlerTests.Handle(query, default);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

        var query = new GetAllTasksQuery { UserId = userClaim };

        _repositoryMock.Setup(r => r.Task.GetAllTasks(false, It.IsAny<CancellationToken>(), userId))
            .ReturnsAsync(new List<CustomTask>());

        _mapperMock.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<Task>>()))
            .Returns(new List<TaskDto>());

        // Act
        var result = await _handlerTests.Handle(query, default);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ThrowsFormatException_WhenUserIdIsInvalid()
    {
        // Arrange
        var invalidClaim = new Claim(ClaimTypes.NameIdentifier, "invalid-guid");
        var query = new GetAllTasksQuery { UserId = invalidClaim };

        // Act
        Func<Task> act = async () => await _handlerTests.Handle(query, default);

        // Assert
        await act.Should().ThrowAsync<FormatException>();
    }

}