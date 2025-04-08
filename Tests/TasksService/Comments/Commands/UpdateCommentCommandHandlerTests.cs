using System.Linq.Expressions;
using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.UseCases.Commands.CommentCommands.UpdateComment;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using TasksService.Domain.Models;

namespace Tests.TasksService.Comments.Commands;

public class UpdateCommentCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly Mock<IValidator<Comment>> _validatorMock = new();
    private readonly UpdateCommentCommandHandler _handler;

    public UpdateCommentCommandHandlerTests()
    {
        _handler = new UpdateCommentCommandHandler(
            _repoMock.Object,
            _mapperMock.Object,
            _cacheMock.Object,
            _validatorMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_UpdateComment_And_ClearCache()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var command = new UpdateCommentCommand
        {
            CommentId = commentId,
            Content = "Updated content"
        };

        var comment = new Comment { Id = commentId, TaskId = taskId };

        _repoMock.Setup(r => r.Comment.FindByCondition(
                It.IsAny<Expression<Func<Comment, bool>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Comment> { comment });

        _mapperMock.Setup(m => m.Map(command, comment)).Verifiable();

        _validatorMock.Setup(v => v.ValidateAsync(comment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _repoMock.Setup(r => r.Comment.Update(comment, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _cacheMock.Verify(c => c.RemoveAsync($"comments: {taskId}"), Times.Once);
    }
}