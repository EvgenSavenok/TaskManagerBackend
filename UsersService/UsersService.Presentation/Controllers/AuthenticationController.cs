using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.DataTransferObjects;
using UsersService.Application.UseCases.Commands.UserCommands.Register;

namespace UsersService.Presentation.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(IMediator mediator) : Controller
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] UserForRegistrationDto userForRegistration)
    {
        var command = new RegisterUserCommand
        {
            UserForRegistrationDto = userForRegistration
        };
        await mediator.Send(command);
        
        return Ok();
    }
}
