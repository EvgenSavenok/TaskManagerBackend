using MediatR;
using UsersService.Application.DataTransferObjects;

namespace UsersService.Application.UseCases.Commands.UserCommands.Authenticate;

public record AuthenticateUserCommand
    : IRequest<(string AccessToken, string RefreshToken)>
{
    public UserForAuthenticationDto UserForAuthenticationDto { get; set; }
}