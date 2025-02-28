using System.Security.Claims;
using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Commands.TaskCommands.CreateTask;
using Application.UseCases.Commands.TaskCommands.DeleteTask;
using Application.UseCases.Commands.TaskCommands.UpdateTask;
using Application.UseCases.Queries.TaskQueries.GetAllTasks;
using Application.UseCases.Queries.TaskQueries.GetTaskById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/tasks")]
[ApiController]
public class TasksController(
    IMediator mediator) : Controller
{
    [HttpGet("getTaskById/{id}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetTask(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTaskByIdQuery(id) { TaskId = id };
        var task = await mediator.Send(query, cancellationToken);
        
        return Ok(task);
    }
    
    [HttpGet("getAllTasksOfUser")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAllTasksOfUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        var query = new GetAllTasksQuery
        {
            UserId = userIdClaim!
        };
        var tasks = await mediator.Send(query);
        
        return Ok(tasks);
    }
    
    [HttpPost("addTask")]
    //[Authorize(Policy = "User")]
    public async Task<IActionResult> CreateTask([FromBody]TaskDto taskDto)
    {
        var command = new CreateTaskCommand
        {
            TaskDto = taskDto
        };
        await mediator.Send(command);
        
        return NoContent();
    }

    [HttpPut("updateTask")]
    //[Authorize(Policy = "User")]
    public async Task<IActionResult> UpdateTask(
        [FromBody] TaskDto taskDto, 
        CancellationToken cancellationToken)
    {
        var command = new UpdateTaskCommand
        {
            TaskDto = taskDto
        };
        await mediator.Send(command, cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("deleteTask/{taskId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> DeleteTask(Guid taskId, CancellationToken cancellationToken)
    {
        var command = new DeleteTaskCommand { TaskId = taskId };
        await mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
}