using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using UsersService.Application.Contracts;
using UsersService.Application.DataTransferObjects;
using UsersService.Domain;
using Microsoft.IdentityModel.Tokens;
using UsersService.Domain.CustomExceptions;

namespace UsersService.Infrastructure;

public class AuthenticationManager(
    UserManager<User> userManager, 
    IConfiguration configuration)
    : IAuthenticationManager
{
    private User _user;

    public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        _user = await userManager.FindByNameAsync(userForAuth.UserName);
        if (_user is null)
            throw new NotFoundException("Cannot find user");
        return await userManager.CheckPasswordAsync(_user, userForAuth.Password);
    }

    public async Task<TokenDto> CreateTokens(User user, bool populateExp)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var refreshTokenLifeTime = jwtSettings.GetSection("RefreshTokenLifeTime").Value;
        if (refreshTokenLifeTime is null)
            throw new NotFoundException("RefreshTokenLifeTime is null");
        int doubleRefreshTokenLifeTime = Int32.Parse(refreshTokenLifeTime);
        
        _user = user;
        
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        
        var refreshToken = GenerateRefreshToken();
        _user.RefreshToken = refreshToken;
        
        if (populateExp)
            _user.RefreshTokenExpireTime = DateTime.UtcNow.AddMinutes(doubleRefreshTokenLifeTime);
        await userManager.UpdateAsync(_user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        
        return new TokenDto(accessToken, refreshToken);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        
        var secretKey = jwtSettings.GetSection("validIssuer").Value;
        
        var key = Encoding.UTF8.GetBytes(secretKey!);
        
        var secret = new SymmetricSecurityKey(key);
        
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    
    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, _user.UserName!),
            new(ClaimTypes.NameIdentifier, _user.Id)
        };
        
        var roles = await userManager.GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        return claims;
    }
    
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken
        (
            issuer: jwtSettings.GetSection("validIssuer").Value,
            audience: jwtSettings.GetSection("validAudience").Value,
            claims: claims,
            expires:
                DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("expires").Value)),
            signingCredentials: signingCredentials
        );
        
        return tokenOptions;
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber);
    }
    
    public async Task<string> CreateAccessToken(User user)
    {
        _user = user;
        
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        
        await userManager.UpdateAsync(_user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        
        return accessToken;
    }
}
