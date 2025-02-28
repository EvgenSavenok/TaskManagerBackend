using MediatR;
using Microsoft.AspNetCore.Identity;
using UsersService.Application.Contracts;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace UsersService.Application.UseCases.Commands.UserCommands.Authenticate;

public class AuthenticateUserCommandHandler(
    UserManager<User> userManager,
    IAuthenticationManager authManager)
    : IRequestHandler<AuthenticateUserCommand, (string AccessToken, string RefreshToken)>
{
    public async Task<(string AccessToken, string RefreshToken)> Handle(
        AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        var userForLogin = request.UserForAuthenticationDto;
        
        var user = await userManager.FindByNameAsync(userForLogin.UserName);
        if (user == null || !await userManager.CheckPasswordAsync(user, userForLogin.Password))
        {
            throw new UnauthorizedException("Cannot find user");
        }
        
        await authManager.ValidateUser(userForLogin);
        
        var tokenDto = await authManager.CreateTokens(user, populateExp: true);
        if (tokenDto.AccessToken == null || tokenDto.RefreshToken == null)
        {
            throw new UnauthorizedException("Cannot create access or refresh token");
        }
        
        return (tokenDto.AccessToken, tokenDto.RefreshToken);
    }
}