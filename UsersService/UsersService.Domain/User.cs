using Microsoft.AspNetCore.Identity;

namespace UsersService.Domain;

public class User: IdentityUser
{
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime RefreshTokenExpireTime { get; set; }
}