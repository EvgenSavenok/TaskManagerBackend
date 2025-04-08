using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.UseCases.Commands.TagCommands.UpdateTag;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Tests.TasksService.Tags.Commands;

public class UpdateTagCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly Mock<IValidator<Tag>> _validatorMock = new();
    private readonly UpdateTagCommandHandler _handler;

    public UpdateTagCommandHandlerTests()
    {
        _handler = new UpdateTagCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _cacheMock.Object,
            _validatorMock.Object
        );
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenTagDoesNotExist()
    {
        var command = new UpdateTagCommand { TagId = Guid.NewGuid(), TagName = "Updated" };
        _repositoryMock.Setup(r => r.Tag.GetTagById(command.TagId, true, default)).ReturnsAsync((Tag)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenInvalid()
    {
        var tag = new Tag { Id = Guid.NewGuid(), Name = "Old" };
        var command = new UpdateTagCommand { TagId = tag.Id, TagName = "New" };

        _repositoryMock.Setup(r => r.Tag.GetTagById(tag.Id, true, default)).ReturnsAsync(tag);
        _mapperMock.Setup(m => m.Map(command, tag));
        _validatorMock.Setup(v => v.ValidateAsync(tag, default)).ReturnsAsync(new ValidationResult
        {
            Errors = { new ValidationFailure("Name", "Invalid name") }
        });

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_UpdatesTagAndRemovesCache_WhenValid()
    {
        var tag = new Tag { Id = Guid.NewGuid(), Name = "Old" };
        var command = new UpdateTagCommand { TagId = tag.Id, TagName = "New" };

        _repositoryMock.Setup(r => r.Tag.GetTagById(tag.Id, true, default)).ReturnsAsync(tag);
        _mapperMock.Setup(m => m.Map(command, tag));
        _validatorMock.Setup(v => v.ValidateAsync(tag, default)).ReturnsAsync(new ValidationResult());

        var result = await _handler.Handle(command, default);

        Assert.Equal(Unit.Value, result);
        _repositoryMock.Verify(r => r.Tag.Update(tag, default), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync("tags: all"), Times.Once);
    }
}