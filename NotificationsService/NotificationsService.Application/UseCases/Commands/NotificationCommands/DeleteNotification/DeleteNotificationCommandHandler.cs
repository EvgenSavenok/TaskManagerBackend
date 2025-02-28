using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Domain.CustomExceptions;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.DeleteNotification;

public class DeleteNotificationCommandHandler(
    IRepositoryManager repository,
    IHangfireService hangfireService) 
    : IRequestHandler<DeleteNotificationCommand>
{
    public async Task<Unit> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var taskId = request.Id;
        
        var notifications = await repository.Notification.FindByCondition(
            notification => notification.TaskId == taskId,
            cancellationToken);

        var notification = notifications.FirstOrDefault();

        if (notification is null)
        {
            throw new NotFoundException($"Уведомление с ID {taskId} не найдено.");
        }
        
        hangfireService.DeleteNotificationInHangfire(notification.HangfireJobId);

        await repository.Notification.Delete(n => n.TaskId == taskId, cancellationToken);
        
        return Unit.Value;
    }
}