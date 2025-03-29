using MediatR;
using Microsoft.AspNetCore.SignalR;
using UsersService.Application.UseCases.Commands.UserCommands.DeleteById;

namespace UsersService.Presentation.SignalRHubs;

public class UserHub(IMediator mediator) : Hub
{
    public async Task DeleteUser(string userId)
    {
        var command = new DeleteUserCommand
        {
            UserId = userId
        };
        await mediator.Send(command);

        await Clients.All.SendAsync("UserDeleted", userId);
    }
}