using Microsoft.AspNetCore.Identity;
using Moq;
using UsersService.Application.UseCases.Queries.UserQueries.GetAllUsers;
using UsersService.Domain;

namespace Tests.UsersService.Queries;

public class GetAllUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUsersInRoleUser()
    {
        // Arrange
        var mockUserManager = MockUserManager();
        var handler = new GetAllUsersQueryHandler(mockUserManager.Object);

        var expectedUsers = new List<User>
        {
            new() { UserName = "user1" },
            new() { UserName = "user2" }
        };

        mockUserManager
            .Setup(m => m.GetUsersInRoleAsync("User"))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await handler.Handle(new GetAllUsersQuery(), default);

        // Assert
        Assert.Equal(expectedUsers.Count, result.Count());
        Assert.Contains(result, u => u.UserName == "user1");
    }

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
    }
}