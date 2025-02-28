using Application.DataTransferObjects.TagsDto;
using Application.UseCases.Commands.TagCommands.CreateTag;
using Application.UseCases.Commands.TagCommands.DeleteTag;
using Application.UseCases.Commands.TagCommands.UpdateTag;
using Application.UseCases.Queries.TagQueries.GetAllTags;
using Application.UseCases.Queries.TagQueries.GetAllTagsOfTask;
using Application.UseCases.Queries.TagQueries.GetTagByTaskId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/tags")]
[ApiController]
public class TagsController(
    IMediator mediator) : Controller
{
    [HttpGet("getTag/{tagId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetTagById(
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var query = new GetTagByIdQuery
        {
            TagId = tagId
        };
        var tag = await mediator.Send(query, cancellationToken);
        return Ok(tag);
    }
    
    [HttpGet("{taskId}/getAllTagsOfTask")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAllTagsOfTask(Guid taskId, CancellationToken cancellationToken)
    {
        var query = new GetAllTagsOfTaskQuery
        {
            TaskId = taskId,
        };
        var taskTags = await mediator.Send(query, cancellationToken);
        return Ok(taskTags);
    }

    [HttpGet("getAllTags")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAllTags(CancellationToken cancellationToken)
    {
        var query = new GetAllTagsQuery();
        var tags = await mediator.Send(query, cancellationToken);
        return Ok(tags);
    }
    
    [HttpPost("addTag")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> AddTag(
        [FromBody]TagDto tagDto, 
        CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand
        {
            TagName = tagDto.TagName
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpPut("updateTag/{tagId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> UpdateTag(
        [FromBody]TagDto tagDto, 
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand
        {
            TagId = tagId,
            TagName = tagDto.TagName
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("deleteTag/{tagId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> DeleteTag(
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand
        {
            TagId = tagId
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}