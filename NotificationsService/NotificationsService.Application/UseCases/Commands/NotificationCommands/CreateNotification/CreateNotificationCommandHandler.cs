using AutoMapper;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;

public class CreateNotificationCommandHandler(
    IRepositoryManager repository,
    IMapper mapper) 
    : IRequestHandler<CreateNotificationCommand>
{
    public async Task<Unit> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationEntity = mapper.Map<Notification>(request);
        
        // TODO
        // Validation of notification entity
        
        await repository.Notification.Create(notificationEntity, cancellationToken);
        
        return Unit.Value;
    }
}