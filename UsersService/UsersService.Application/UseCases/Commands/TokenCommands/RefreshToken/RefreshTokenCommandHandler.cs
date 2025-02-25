using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UsersService.Application.Contracts;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace UsersService.Application.UseCases.Commands.TokenCommands.RefreshToken;

public class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    IAuthenticationManager authManager,
    IConfiguration configuration)
    : IRequestHandler<RefreshTokenCommand, string>
{
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetSection("validIssuer").Value;
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            ValidateLifetime = false,
            ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
            ValidAudience = jwtSettings.GetSection("validAudience").Value,
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid Token");
        }
        return principal;
    }
    
    public async Task<string> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenDto = request.TokenDto;
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var user = await userManager.FindByNameAsync(principal.Identity.Name);
        if (user is null || user.RefreshToken != tokenDto.RefreshToken ||
            user.RefreshTokenExpireTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token or this token expired.");
        }
        string token = await authManager.CreateAccessToken(user);
        return token;
    }
}