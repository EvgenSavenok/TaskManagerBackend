using AutoMapper;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Domain.CustomExceptions;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.DeleteNotification;

public class DeleteNotificationCommandHandler(
    IRepositoryManager repository) 
    : IRequestHandler<DeleteNotificationCommand>
{
    public async Task<Unit> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationId = request.Id;

        await repository.Notification.Delete(n => n.Id == notificationId, cancellationToken);
        
        return Unit.Value;
    }
}