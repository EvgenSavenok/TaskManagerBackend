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
    [HttpGet("getTaskById/{id}")]
    public async Task<IActionResult> GetTask(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTaskByIdQuery(id) { TaskId = id };
        var task = await mediator.Send(query, cancellationToken);
        return Ok(task);
    }
    
    [HttpGet("getAllTasks")]
    public async Task<IActionResult> GetAllTasks()
    {
        var query = new GetAllTasksQuery();
        var tasks = await mediator.Send(query);
        return Ok(tasks);
    }
    
    [HttpPost("addTask")]
    public async Task<IActionResult> CreateTask([FromBody]TaskDto taskDto)
    {
        var command = new CreateTaskCommand { TaskDto = taskDto };
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPut("updateTask")]
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
    public async Task<IActionResult> DeleteTask(Guid taskId, CancellationToken cancellationToken)
    {
        var command = new DeleteTaskCommand { TaskId = taskId };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}