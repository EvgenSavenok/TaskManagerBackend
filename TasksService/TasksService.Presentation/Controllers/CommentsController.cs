using Application.DataTransferObjects.CommentsDto;
using Application.UseCases.Commands.CommentCommands.CreateComment;
using Application.UseCases.Commands.CommentCommands.DeleteComment;
using Application.UseCases.Commands.CommentCommands.UpdateComment;
using Application.UseCases.Queries.CommentQueries.GetAllCommentsOfTask;
using Application.UseCases.Queries.CommentQueries.GetCommentById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentsController(
    IMediator mediator) : Controller
{
    [HttpGet("getComment/{commentId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetCommentById(
        Guid commentId,
        CancellationToken cancellationToken)
    {
        var query = new GetCommentByIdQuery
        {
            CommentId = commentId
        };
        var comment = await mediator.Send(query, cancellationToken);
        return Ok(comment);
    }
    
    [HttpGet("{taskId}/getAllCommentsOfTask")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAllCommentsOfTask(
        Guid taskId, 
        CancellationToken cancellationToken)
    {
        var query = new GetAllCommentsQuery
        {
            TaskId = taskId
        };
        var tags = await mediator.Send(query, cancellationToken);
        return Ok(tags);
    }
    
    [HttpPost("addComment")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> AddComment(
        [FromBody]CommentDto commentDto, 
        CancellationToken cancellationToken)
    {
        var command = new CreateCommentCommand
        {
            TaskId = commentDto.TaskId,
            Content = commentDto.Content
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpPut("updateComment")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> UpdateComment(
        [FromBody]CommentDto commentDto, 
        CancellationToken cancellationToken)
    {
        var command = new UpdateCommentCommand
        {
            CommentId = commentDto.CommentId,
            Content = commentDto.Content
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("deleteComment/{commentId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> DeleteComment(
        Guid commentId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCommentCommand
        {
            CommentId = commentId
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}