using Application.DataTransferObjects.CommentsDto;
using Application.UseCases.Commands.CommentCommands.CreateComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentsController(
    IMediator mediator) : Controller
{
    [HttpPost("addComment")]
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

}