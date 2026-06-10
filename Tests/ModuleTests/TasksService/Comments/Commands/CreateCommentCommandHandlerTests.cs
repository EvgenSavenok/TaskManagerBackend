using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.DataTransferObjects.CommentsDto;
using Application.UseCases.Commands.CommentCommands.CreateComment;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using TasksService.Domain.Models;

namespace ModuleTests.TasksService.Comments.Commands;

public class CreateCommentCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly Mock<IValidator<Comment>> _validatorMock = new();
    private readonly CreateCommentCommandHandler _handler;

    public CreateCommentCommandHandlerTests()
    {
        _handler = new CreateCommentCommandHandler(
            _repoMock.Object,
            _mapperMock.Object,
            _cacheMock.Object,
            _validatorMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_CreateComment_And_ClearCache()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Content = "Test comment"
        };

        var comment = new Comment { Id = command.Id, TaskId = command.TaskId, Content = command.Content };
        var task = new CustomTask { Id = command.TaskId };

        _mapperMock.Setup(m => m.Map<Comment>(command)).Returns(comment);
        _validatorMock.Setup(v => v.ValidateAsync(comment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoMock.Setup(r => r.Task.GetTaskByIdAsync(command.TaskId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _repoMock.Setup(r => r.Comment.Create(comment, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<CommentDto>(comment)).Returns(new CommentDto { CommentId = comment.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.CommentId.Should().Be(command.Id);
        _cacheMock.Verify(c => c.RemoveAsync($"comments: {comment.TaskId}"), Times.Once);
    }
}