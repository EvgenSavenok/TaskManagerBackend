using AutoMapper;
using FluentValidation;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Domain.CustomExceptions;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.UpdateNotification;

public class UpdateNotificationCommandHandler(
    IRepositoryManager repository,
    IValidator<Notification> validator,
    IMapper mapper) 
    : IRequestHandler<UpdateNotificationCommand>
{
    public async Task<Unit> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationDto = request.NotificationDto;
        
        var notifications = await repository.Notification.FindByCondition(
            c => c.Id == notificationDto.Id,
            cancellationToken);
        var notificationEntity = notifications.FirstOrDefault();
        
        if (notificationEntity == null)
        {
            throw new NotFoundException($"Notification with id {notificationDto.Id} not found");
        }
        
        mapper.Map(request, notificationEntity);
        
        var validationResult = await validator.ValidateAsync(notificationEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await repository.Notification.Update(notificationEntity, cancellationToken);
        
        return Unit.Value;
    }
}