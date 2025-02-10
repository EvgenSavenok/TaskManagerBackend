using Application.DataTransferObjects.TasksDto;
using Application.UseCases.Commands.TaskCommands.CreateTask;
using Application.UseCases.Commands.TaskCommands.DeleteTask;
using Application.UseCases.Commands.TaskCommands.UpdateTask;
using Application.UseCases.Queries.TaskQueries.GetAllTasks;
using Application.UseCases.Queries.TaskQueries.GetTaskById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/tasks")]
[ApiController]
public class TasksController(
    IMediator mediator) : Controller
{
    [HttpPost("addTask")]
    public async Task<IActionResult> CreateTask([FromBody]TaskDto taskDto)
    {
        var command = new CreateTaskCommand { TaskDto = taskDto };
        await mediator.Send(command);
        return Ok();
    }

    [HttpGet("getAllTasks")]
    public async Task<IActionResult> GetAllTasks()
    {
        var query = new GetAllTasksQuery { };
        var tasks = await mediator.Send(query);
        return Ok(tasks);
    }
    
    [HttpGet("getTaskById/{id}")]
    public async Task<IActionResult> GetTask(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTaskByIdQuery(id) { TaskId = id };
        var task = await mediator.Send(query, cancellationToken);
        return Ok(task);
    }

    [HttpPut("updateTask")]
    public async Task<IActionResult> UpdateTask([FromBody] TaskDto taskDto, CancellationToken cancellationToken)
    {
        var command = new UpdateTaskCommand { TaskDto = taskDto };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("deleteTask/{id}")]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTaskCommand() { TaskId = id };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}