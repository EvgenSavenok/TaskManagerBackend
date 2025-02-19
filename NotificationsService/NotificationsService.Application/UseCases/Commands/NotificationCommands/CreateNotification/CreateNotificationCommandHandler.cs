using AutoMapper;
using FluentValidation;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;

public class CreateNotificationCommandHandler(
    IRepositoryManager repository,
    IValidator<Notification> validator,
    IMapper mapper,
    IHangfireService hangfireService) 
    : IRequestHandler<CreateNotificationCommand, Notification>
{
    public async Task<Notification> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationEntity = mapper.Map<Notification>(request);
        
        var validationResult = await validator.ValidateAsync(notificationEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        notificationEntity.Deadline = DateTime.SpecifyKind(notificationEntity.Deadline, DateTimeKind.Local);
        notificationEntity.Deadline = TimeZoneInfo.ConvertTimeToUtc(notificationEntity.Deadline);
        
        var jobId = hangfireService.ScheduleNotificationInHangfire(notificationEntity, cancellationToken);
        
        notificationEntity.HangfireJobId = jobId;
        
        await repository.Notification.Create(notificationEntity, cancellationToken);
        
        return notificationEntity;
    }
}