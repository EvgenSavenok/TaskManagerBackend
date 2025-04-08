using System.Linq.Expressions;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.UseCases.Commands.CommentCommands.DeleteComment;
using FluentAssertions;
using MediatR;
using Moq;
using TasksService.Domain.Models;

namespace Tests.TasksService.Comments.Commands;

public class DeleteCommentCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repoMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly DeleteCommentCommandHandler _handler;

    public DeleteCommentCommandHandlerTests()
    {
        _handler = new DeleteCommentCommandHandler(_repoMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_Should_DeleteComment_And_ClearCache()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var comment = new Comment { Id = commentId, TaskId = taskId };
        _repoMock.Setup(r => r.Comment.FindByCondition(
                It.IsAny<Expression<Func<Comment, bool>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Comment> { comment });

        _repoMock.Setup(r => r.Comment.Delete(comment, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new DeleteCommentCommand { CommentId = commentId }, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _cacheMock.Verify(c => c.RemoveAsync($"comments: {taskId}"), Times.Once);
    }

}