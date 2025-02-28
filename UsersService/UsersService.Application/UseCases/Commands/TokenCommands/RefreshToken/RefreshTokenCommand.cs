using MediatR;
using UsersService.Application.DataTransferObjects;

namespace UsersService.Application.UseCases.Commands.TokenCommands.RefreshToken;

public record RefreshTokenCommand : IRequest<string>
{
    public TokenDto TokenDto { get; set; }
}