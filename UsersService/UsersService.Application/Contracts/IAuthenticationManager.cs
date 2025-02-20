using UsersService.Application.DataTransferObjects;
using UsersService.Domain;

namespace UsersService.Application.Contracts;

public interface IAuthenticationManager
{
    Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
    
    Task<TokenDto> CreateTokens(User user, bool populateExp);
}
