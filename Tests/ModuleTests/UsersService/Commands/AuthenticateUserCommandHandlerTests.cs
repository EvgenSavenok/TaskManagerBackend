using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using TasksService.Domain.Models;
using UsersService.Application.Contracts;
using UsersService.Application.DataTransferObjects;
using UsersService.Application.UseCases.Commands.UserCommands.Authenticate;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace ModuleTests.UsersService.Commands;

public class AuthenticateUserCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IAuthenticationManager> _authManagerMock;
    private readonly AuthenticateUserCommandHandler _handler;

    public AuthenticateUserCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _authManagerMock = new Mock<IAuthenticationManager>();
        _handler = new AuthenticateUserCommandHandler(_userManagerMock.Object, _authManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAccessToken_AndSetsCookie()
    {
        // Arrange
        var user = new User { UserName = "user" };
        var dto = new UserForAuthenticationDto { UserName = "user", Password = "pass" };

        var cookiesMock = new Mock<IResponseCookies>();
        var responseMock = new Mock<HttpResponse>();
        var httpContextMock = new Mock<HttpContext>();

        responseMock.SetupGet(r => r.Cookies).Returns(cookiesMock.Object);
        httpContextMock.SetupGet(c => c.Response).Returns(responseMock.Object);

        _userManagerMock.Setup(m => m.FindByNameAsync(dto.UserName)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _authManagerMock.Setup(m => m.ValidateUser(dto)).ReturnsAsync(true);
        _authManagerMock.Setup(m => m.CreateTokens(user, true))
            .ReturnsAsync(new TokenDto("access-token", "refresh-token"));

        var command = new AuthenticateUserCommand
        {
            UserForAuthenticationDto = dto,
            HttpContext = httpContextMock.Object
        };

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Should().Be("access-token");

        cookiesMock.Verify(c => c.Append(
            "refreshToken",
            "refresh-token",
            It.IsAny<CookieOptions>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ThrowsUnauthorizedException()
    {
        var dto = new UserForAuthenticationDto { UserName = "wrong", Password = "pass" };
        _userManagerMock.Setup(m => m.FindByNameAsync(dto.UserName)).ReturnsAsync((User)null);

        var command = new AuthenticateUserCommand
        {
            UserForAuthenticationDto = dto,
            HttpContext = new DefaultHttpContext()
        };

        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, default));
    }
}