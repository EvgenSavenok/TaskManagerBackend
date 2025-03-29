using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UsersService.Application.DataTransferObjects;
using UsersService.Application.UseCases.Commands.UserCommands.Authenticate;
using UsersService.Application.UseCases.Commands.UserCommands.DeleteById;
using UsersService.Application.UseCases.Commands.UserCommands.Register;
using UsersService.Application.UseCases.Queries.UserQueries.GetAllUsers;
using UsersService.Presentation.SignalRHubs;

namespace UsersService.Presentation.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController(
    IMediator mediator,
    IHubContext<UserHub> hubContext) : Controller
{
    [HttpGet("getAllUsers")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var query = new GetAllUsersQuery();
        var users = await mediator.Send(query);
        
        return Ok(users);
    }
    
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
    
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForLogin)
    {
        var command = new AuthenticateUserCommand
        {
            UserForAuthenticationDto = userForLogin,
            HttpContext = HttpContext
        };
        var accessToken = await mediator.Send(command);
         
        return Ok(new { AccessToken = accessToken });
    }

    [HttpDelete("deleteUser/{userId}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var command = new DeleteUserCommand
        {
            UserId = userId
        };
        await mediator.Send(command);
        
        await hubContext.Clients.All.SendAsync("UserDeleted", userId);
        
        return Ok();
    }
}
