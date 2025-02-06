using Application.Contracts.UseCasesContracts;
using Application.DataTransferObjects.TasksDto;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/tasks")]
[ApiController]
public class TasksController : Controller
{
    private ICreateTaskUseCase _createTaskUseCase;
    public TasksController(ICreateTaskUseCase createTaskUseCase)
    {
        _createTaskUseCase = createTaskUseCase;
    }
    
    [HttpPost("addTask")]
    public async Task<IActionResult> CreateTask([FromBody]TaskForCreationDto taskDto)
    {
        await _createTaskUseCase.ExecuteAsync(taskDto);   
        return Ok();
    }
}