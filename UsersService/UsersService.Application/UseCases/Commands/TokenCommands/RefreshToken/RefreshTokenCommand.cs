using MediatR;
using Microsoft.AspNetCore.Http;
using UsersService.Application.DataTransferObjects;

namespace UsersService.Application.UseCases.Commands.TokenCommands.RefreshToken;

public record RefreshTokenCommand : IRequest<string>
{
    public string AccessToken { get; set; }
    
    public HttpContext HttpContext { get; set; }
}