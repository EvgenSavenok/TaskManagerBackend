using AutoMapper;
using FluentValidation;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Domain.Enums;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.UpdateNotification;

public class UpdateNotificationCommandHandler(
    IRepositoryManager repository,
    IValidator<Notification> validator,
    IMapper mapper,
    IHangfireService hangfireService) 
    : IRequestHandler<UpdateNotificationCommand>
{
    public async Task<Unit> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var taskId = request.NotificationDto.TaskId.ToString();
        var notifications = await repository.Notification.FindByCondition(
            c => c.TaskId.ToString() == taskId, cancellationToken);
        var notificationEntity = notifications.FirstOrDefault();

        DateTime oldDeadline = notificationEntity!.Deadline;
        
        mapper.Map(request, notificationEntity);

        var validationResult = await validator.ValidateAsync(notificationEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors.ToString());
        }
        
        notificationEntity.Status = Status.Unsent; 
        
        if (!oldDeadline.Equals(notificationEntity.Deadline))
        {
            notificationEntity.Deadline = DateTime.SpecifyKind(notificationEntity.Deadline, DateTimeKind.Utc);
            notificationEntity.Deadline = TimeZoneInfo.ConvertTimeToUtc(notificationEntity.Deadline);

            hangfireService.ScheduleNotificationInHangfire(notificationEntity, cancellationToken);
        }
        
        await repository.Notification.UpdateNotificationByTaskId(notificationEntity, cancellationToken);
        
        return Unit.Value;
    }
}