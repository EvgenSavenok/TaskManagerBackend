using MediatR;
using Microsoft.AspNetCore.Identity;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace UsersService.Application.UseCases.Queries.UserQueries.GetUserById;

public class GetUserByIdQueryHandler(
    UserManager<User> userManager) 
    : IRequestHandler<GetUserByIdQuery, User>
{
    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        
        return user;
    }
}