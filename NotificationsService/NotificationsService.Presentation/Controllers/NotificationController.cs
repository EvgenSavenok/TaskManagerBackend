using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationsService.Application.DataTransferObjects.NotificationsDto;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;

namespace NotificationsService.Presentation.Controllers;

[Route("api/notifications")]
[ApiController]
public class NotificationController(
    IMediator mediator) : Controller
{
    [HttpPost("addNotification")]
    public async Task<IActionResult> AddNotification(
        [FromBody] NotificationDto notificationDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateNotificationCommand
        {
           NotificationDto = notificationDto
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}