using Application.Contracts.Redis;
using Application.Contracts.RepositoryContracts;
using Application.UseCases.Commands.TagCommands.CreateTag;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using TasksService.Domain.CustomExceptions;
using TasksService.Domain.Models;

namespace Tests.TasksService.Tags.Commands;

public class CreateTagCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRedisCacheService> _cacheMock = new();
    private readonly Mock<IValidator<Tag>> _validatorMock = new();

    private readonly CreateTagCommandHandler _handler;

    public CreateTagCommandHandlerTests()
    {
        _handler = new CreateTagCommandHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _cacheMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenInvalid()
    {
        var command = new CreateTagCommand { TagName = "Invalid" };
        var tag = new Tag { Name = "Invalid" };

        _mapperMock.Setup(m => m.Map<Tag>(command)).Returns(tag);
        _validatorMock.Setup(v => v.ValidateAsync(tag, default))
            .ReturnsAsync(new ValidationResult { Errors = { new ValidationFailure("Name", "Invalid name") } });

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ThrowsAlreadyExistsException_WhenTagExists()
    {
        var command = new CreateTagCommand { TagName = "Work" };
        var tag = new Tag { Name = "Work" };

        _mapperMock.Setup(m => m.Map<Tag>(command)).Returns(tag);
        _validatorMock.Setup(v => v.ValidateAsync(tag, default)).ReturnsAsync(new ValidationResult());
        _repositoryMock.Setup(r => r.Tag.GetTagByName("Work", false, default)).ReturnsAsync(new Tag());

        await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_CreatesTagAndRemovesCache_WhenValid()
    {
        var command = new CreateTagCommand { TagName = "New" };
        var tag = new Tag { Name = "New" };

        _mapperMock.Setup(m => m.Map<Tag>(command)).Returns(tag);
        _validatorMock.Setup(v => v.ValidateAsync(tag, default)).ReturnsAsync(new ValidationResult());
        _repositoryMock.Setup(r => r.Tag.GetTagByName("New", false, default)).ReturnsAsync((Tag)null);

        var result = await _handler.Handle(command, default);

        Assert.Equal(Unit.Value, result);
        _repositoryMock.Verify(r => r.Tag.Create(tag, default), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync("tags: all"), Times.Once);
    }
}