using Application.DataTransferObjects.TagsDto;
using Application.UseCases.Commands.TagCommands.CreateTag;
using Application.UseCases.Commands.TagCommands.DeleteTag;
using Application.UseCases.Commands.TagCommands.UpdateTag;
using Application.UseCases.Queries.TagQueries.GetAllTags;
using Application.UseCases.Queries.TagQueries.GetTagByTaskId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/tags")]
[ApiController]
public class TagsController(
    IMediator mediator) : Controller
{
    [HttpGet("{taskId}/getTag/{tagId}")]
    public async Task<IActionResult> GetTagByTaskId(
        Guid taskId,
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var query = new GetTagByTaskIdQuery(taskId)
        {
            TaskId = taskId,
            TagId = tagId
        };
        var tag = await mediator.Send(query, cancellationToken);
        return Ok(tag);
    }
    
    [HttpGet("{taskId}/getAllTags")]
    public async Task<IActionResult> GetAllTags(Guid taskId, CancellationToken cancellationToken)
    {
        var query = new GetAllTagsQuery()
        {
            TaskId = taskId,
        };
        var tags = await mediator.Send(query, cancellationToken);
        return Ok(tags);
    }
    
    [HttpPost("{taskId}/addTag")]
    public async Task<IActionResult> AddTagToTask(
        [FromBody]TagDto tagDto, 
        Guid taskId, 
        CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand
        {
            TaskId = taskId,
            TagName = tagDto.TagName
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpPut("{taskId}/updateTag/{tagId}")]
    public async Task<IActionResult> UpdateTag(
        [FromBody]TagDto tagDto, 
        Guid taskId,
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand
        {
            TaskId = taskId,
            TagId = tagId,
            TagName = tagDto.TagName
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("{taskId}/deleteTag/{tagId}")]
    public async Task<IActionResult> DeleteTag(
        Guid taskId, 
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand
        {
            TaskId = taskId,
            TagId = tagId
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}