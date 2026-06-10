using MediatR;

namespace UsersService.Application.UseCases.Commands.UserCommands.DeleteById;

public record DeleteUserCommand : IRequest<Unit>
{ 
    public string UserId { get; set; }
}