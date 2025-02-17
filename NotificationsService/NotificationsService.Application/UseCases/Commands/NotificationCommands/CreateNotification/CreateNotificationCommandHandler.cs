using AutoMapper;
using FluentValidation;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;

public class CreateNotificationCommandHandler(
    IRepositoryManager repository,
    IValidator<Notification> validator,
    IMapper mapper) 
    : IRequestHandler<CreateNotificationCommand>
{
    public async Task<Unit> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationEntity = mapper.Map<Notification>(request);
        
        var validationResult = await validator.ValidateAsync(notificationEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        await repository.Notification.Create(notificationEntity, cancellationToken);
        
        return Unit.Value;
    }
}