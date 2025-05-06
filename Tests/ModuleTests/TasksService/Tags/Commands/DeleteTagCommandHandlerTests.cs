using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.UseCases.Commands.TagCommands.DeleteTag;
using MediatR;
using Moq;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace ModuleTests.TasksService.Tags.Commands;

public class DeleteTagCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly DeleteTagCommandHandler _handler;

    public DeleteTagCommandHandlerTests()
    {
        _handler = new DeleteTagCommandHandler(_repositoryMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenTagDoesNotExist()
    {
        var command = new DeleteTagCommand { TagId = Guid.NewGuid() };
        _repositoryMock.Setup(r => r.Tag.GetTagById(command.TagId, true, default)).ReturnsAsync((Tag)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_DeletesTagAndRemovesCache_WhenTagExists()
    {
        var tag = new Tag { Id = Guid.NewGuid(), Name = "Urgent" };
        var command = new DeleteTagCommand { TagId = tag.Id };

        _repositoryMock.Setup(r => r.Tag.GetTagById(tag.Id, true, default)).ReturnsAsync(tag);

        var result = await _handler.Handle(command, default);

        Assert.Equal(Unit.Value, result);
        _repositoryMock.Verify(r => r.Tag.Delete(tag, default), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync("tags: all"), Times.Once);
    }
}