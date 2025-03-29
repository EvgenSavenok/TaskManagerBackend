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
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.AccessToken))
        {
            return BadRequest("AccessToken is required.");
        }
        
        var command = new RefreshTokenCommand
        {
            AccessToken = request.AccessToken,
            HttpContext = HttpContext
        };
        var refreshedAccessToken = await mediator.Send(command);
        
        return Ok(refreshedAccessToken);
    }
}
