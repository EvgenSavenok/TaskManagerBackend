using Microsoft.AspNetCore.Identity;
using Moq;
using UsersService.Application.UseCases.Queries.UserQueries.GetUserById;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace ModuleTests.UsersService.Queries;

public class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var mockUserManager = MockUserManager();
        var user = new User { Id = "123", UserName = "testuser" };
        mockUserManager
            .Setup(m => m.FindByIdAsync("123"))
            .ReturnsAsync(user);

        var handler = new GetUserByIdQueryHandler(mockUserManager.Object);

        // Act
        var result = await handler.Handle(new GetUserByIdQuery { UserId = "123" }, default);

        // Assert
        Assert.Equal("testuser", result.UserName);
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var mockUserManager = MockUserManager();
        mockUserManager
            .Setup(m => m.FindByIdAsync("999"))
            .ReturnsAsync((User?)null);

        var handler = new GetUserByIdQueryHandler(mockUserManager.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetUserByIdQuery { UserId = "999" }, default));
    }

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }
}