using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using UsersService.Application.Contracts;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace UsersService.Application.UseCases.Commands.UserCommands.Authenticate;

public class AuthenticateUserCommandHandler(
    UserManager<User> userManager,
    IAuthenticationManager authManager)
    : IRequestHandler<AuthenticateUserCommand, string>
{
    public async Task<string> Handle(
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
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        request.HttpContext.Response.Cookies.Append("refreshToken", tokenDto.RefreshToken, cookieOptions);
        
        return tokenDto.AccessToken;
    }
}