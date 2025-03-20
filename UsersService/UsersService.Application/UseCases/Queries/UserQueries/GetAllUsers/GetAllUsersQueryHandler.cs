using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersService.Domain;

namespace UsersService.Application.UseCases.Queries.UserQueries.GetAllUsers;

public class GetAllUsersQueryHandler(
    UserManager<User> userManager) 
    : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
{
    public async Task<IEnumerable<User>> Handle(
        GetAllUsersQuery request, 
        CancellationToken cancellationToken)
    {
        var users = await userManager.GetUsersInRoleAsync("User");
        
        return users;
    }
}