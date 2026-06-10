using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using UsersService.Application.UseCases.Commands.UserCommands.DeleteById;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace ModuleTests.UsersService.Commands;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _handler = new DeleteUserCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_UserExists_DeletesUser()
    {
        var user = new User { Id = "123" };
        _userManagerMock.Setup(x => x.FindByIdAsync("123")).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var command = new DeleteUserCommand { UserId = "123" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ThrowsNotFoundException()
    {
        _userManagerMock.Setup(x => x.FindByIdAsync("notfound")).ReturnsAsync((User)null);

        var command = new DeleteUserCommand { UserId = "notfound" };

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}