using MediatR;
using Microsoft.AspNetCore.Identity;
using UsersService.Application.DataTransferObjects;

namespace UsersService.Application.UseCases.Commands.UserCommands.Register;

public record RegisterUserCommand : IRequest<IdentityResult>
{
    public UserForRegistrationDto UserForRegistrationDto { get; set; }
}