using MediatR;
using Microsoft.AspNetCore.Http;
using UsersService.Application.DataTransferObjects;

namespace UsersService.Application.UseCases.Commands.UserCommands.Authenticate;

public record AuthenticateUserCommand
    : IRequest<string>
{
    public UserForAuthenticationDto UserForAuthenticationDto { get; set; }
    
    public HttpContext HttpContext { get; set; } = null!;
}