using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.DataTransferObjects;
using UsersService.Application.UseCases.Commands.TokenCommands.RefreshToken;

namespace UsersService.Presentation.Controllers;

[Route("api/token")]
[ApiController]
public class TokenController(
    IMediator mediator) : Controller
{
    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
    {
        var command = new RefreshTokenCommand
        {
            TokenDto = tokenDto
        };
        var accessToken = await mediator.Send(command);
        
        return Ok(accessToken);
    }
}
