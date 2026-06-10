using MediatR;
using Microsoft.AspNetCore.Identity;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace UsersService.Application.UseCases.Commands.UserCommands.DeleteById;

public class DeleteUserCommandHandler(UserManager<User> userManager)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        await userManager.DeleteAsync(user);

        return Unit.Value;
    }
}