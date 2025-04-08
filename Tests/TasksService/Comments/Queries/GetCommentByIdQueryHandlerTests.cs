using System.Linq.Expressions;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.CommentsDto;
using Application.UseCases.Queries.CommentQueries.GetCommentById;
using AutoMapper;
using Moq;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Tests.TasksService.Comments.Queries;

public class GetCommentByIdQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetCommentByIdQueryHandler _handler;

    public GetCommentByIdQueryHandlerTests()
    {
        _handler = new GetCommentByIdQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCachedComment_IfCacheExists()
    {
        var commentId = Guid.NewGuid();
        var cachedComment = new CommentDto { Content = "Cached" };

        _cacheMock.Setup(c => c.GetAsync<CommentDto>($"comment: {commentId}"))
                  .ReturnsAsync(cachedComment);

        var result = await _handler.Handle(new GetCommentByIdQuery { CommentId = commentId }, default);

        Assert.Equal(cachedComment, result);
        _repositoryMock.Verify(r => r.Comment.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>(), false, default), Times.Never);
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenCommentNotFound()
    {
        var commentId = Guid.NewGuid();

        _cacheMock.Setup(c => c.GetAsync<CommentDto>($"comment: {commentId}")).ReturnsAsync((CommentDto)null);
        _repositoryMock.Setup(r => r.Comment.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>(), false, default))
                       .ReturnsAsync(new List<Comment>());

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new GetCommentByIdQuery { CommentId = commentId }, default));
    }

    [Fact]
    public async Task Handle_ReturnsMappedComment_AndCachesIt_IfNotCached()
    {
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, Content = "From DB" };
        var commentDto = new CommentDto { Content = "From DB" };

        _cacheMock.Setup(c => c.GetAsync<CommentDto>($"comment: {commentId}")).ReturnsAsync((CommentDto)null);
        _repositoryMock.Setup(r => r.Comment.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>(), false, default))
                       .ReturnsAsync(new List<Comment> { comment });

        _mapperMock.Setup(m => m.Map<CommentDto>(comment)).Returns(commentDto);

        var result = await _handler.Handle(new GetCommentByIdQuery { CommentId = commentId }, default);

        Assert.Equal(commentDto, result);
        _cacheMock.Verify(c => c.SetAsync($"comment: {commentId}", commentDto, It.IsAny<TimeSpan>()), Times.Once);
    }
}