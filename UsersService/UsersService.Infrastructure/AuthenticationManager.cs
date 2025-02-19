using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using UsersService.Application.Contracts;
using UsersService.Application.DataTransferObjects;
using UsersService.Domain;

namespace UsersService.Infrastructure;

public class AuthenticationManager(UserManager<User> userManager, IConfiguration configuration)
    : IAuthenticationManager
{
    private User _user;

    public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        _user = await userManager.FindByNameAsync(userForAuth.UserName);
        return _user != null && await userManager.CheckPasswordAsync(_user,
            userForAuth.Password);
    }
}
