using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Commands.TaskCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/tasks")]
[ApiController]
public class TasksController(
    IMediator mediator) : Controller
{
    [HttpPost("addTask")]
    public async Task<IActionResult> CreateTask([FromBody]CreateTaskCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}