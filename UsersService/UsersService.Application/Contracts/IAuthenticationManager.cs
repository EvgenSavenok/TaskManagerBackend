using UsersService.Application.DataTransferObjects;

namespace UsersService.Application.Contracts;

public interface IAuthenticationManager
{
    Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
}
