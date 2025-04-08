using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using StackExchange.Redis;
using UsersService.Application.DataTransferObjects;
using UsersService.Application.UseCases.Commands.UserCommands.Register;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace Tests.UsersService.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        _handler = new RegisterUserCommandHandler(_mapperMock.Object, _userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_NewUser_CreatesUserAndAssignsRole()
    {
        var dto = new UserForRegistrationDto
        {
            UserName = "newuser", Email = "new@x.com", Password = "123456", Role = UserForRegistrationDto.UserRole.User
        };
        var user = new User { UserName = dto.UserName };

        _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
        _userManagerMock.Setup(u => u.FindByNameAsync(dto.UserName)).ReturnsAsync((User)null);
        _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync((User)null);
        _userManagerMock.Setup(u => u.CreateAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock
            .Setup(u => u.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        var command = new RegisterUserCommand { UserForRegistrationDto = dto };

        var result = await _handler.Handle(command, default);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ExistingUsername_ThrowsAlreadyExistsException()
    {
        var dto = new UserForRegistrationDto { UserName = "exist", Email = "email", Password = "pass" };
        var user = new User { UserName = dto.UserName };
        
        _userManagerMock.Setup(u => u.FindByNameAsync(dto.UserName)).ReturnsAsync(user);

        var command = new RegisterUserCommand { UserForRegistrationDto = dto };

        await Assert.ThrowsAsync<AlreadyExistsException>(() => _handler.Handle(command, default));
    }
}