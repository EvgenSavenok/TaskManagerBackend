using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UsersService.Domain;
using UsersService.Domain.CustomExceptions;

namespace UsersService.Application.UseCases.Commands.UserCommands.Register;

public class RegisterUserCommandHandler(IMapper mapper, UserManager<User> userManager)
    : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userDto = request.UserForRegistrationDto;
        
        var user = mapper.Map<User>(userDto);
        
        var existingUser = await userManager.FindByNameAsync(userDto.UserName);
        if (existingUser != null)
        {
            throw new AlreadyExistsException("User with such username already exists.");
        }
        
        var existingEmail = await userManager.FindByEmailAsync(userDto.Email);
        if (existingEmail != null)
        {
            throw new AlreadyExistsException("User with such email already exists.");
        }
        
        var result = await userManager.CreateAsync(user, userDto.Password);
        if (result.Succeeded)
        {
            var userRoleAsString = userDto.Role.ToString();
            await userManager.AddToRolesAsync(user, new List<string> { userRoleAsString });
        }
        
        return result;
    }
}
