using System.Linq.Expressions;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.CommentsDto;
using Application.UseCases.Queries.CommentQueries.GetAllCommentsOfTask;
using AutoMapper;
using Moq;
using TasksService.Domain.Models;

namespace ModuleTests.TasksService.Comments.Queries;

public class GetAllCommentsOfTaskQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetAllCommentsOfTaskQueryHandler _handler;

    public GetAllCommentsOfTaskQueryHandlerTests()
    {
        _handler = new GetAllCommentsOfTaskQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCachedComments_IfCacheExists()
    {
        var taskId = Guid.NewGuid();
        var cachedComments = new List<CommentDto> { new() { Content = "Cached comment" } };

        _cacheMock.Setup(c => c.GetAsync<IEnumerable<CommentDto>>($"comments: {taskId}"))
                  .ReturnsAsync(cachedComments);

        var result = await _handler.Handle(new GetAllCommentsQuery { TaskId = taskId }, default);

        Assert.Equal(cachedComments, result);
        _repositoryMock.Verify(r => r.Comment.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>(), false, default), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsMappedComments_AndCachesThem_IfNotCached()
    {
        var taskId = Guid.NewGuid();
        var comments = new List<Comment> { new() { Id = Guid.NewGuid(), TaskId = taskId, Content = "New comment" } };
        var commentDtos = new List<CommentDto> { new() { Content = "New comment" } };

        _cacheMock.Setup(c => c.GetAsync<IEnumerable<CommentDto>>($"comments: {taskId}"))
                  .ReturnsAsync((IEnumerable<CommentDto>)null);

        _repositoryMock.Setup(r => r.Comment.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>(), false, default))
                       .ReturnsAsync(comments);

        _mapperMock.Setup(m => m.Map<IEnumerable<CommentDto>>(comments)).Returns(commentDtos);

        var result = await _handler.Handle(new GetAllCommentsQuery { TaskId = taskId }, default);

        Assert.Equal(commentDtos, result);
        _cacheMock.Verify(c => c.SetAsync(
                "comments: " + taskId,
                It.IsAny<IEnumerable<CommentDto>>(),
                It.IsAny<TimeSpan>()),
            Times.Once);

    }
}