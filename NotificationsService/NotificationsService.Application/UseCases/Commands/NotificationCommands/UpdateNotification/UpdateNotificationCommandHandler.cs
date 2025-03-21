using AutoMapper;
using FluentValidation;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Application.UseCases.Commands.NotificationCommands.CreateNotification;
using NotificationsService.Domain.CustomExceptions;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.UseCases.Commands.NotificationCommands.UpdateNotification;

public class UpdateNotificationCommandHandler(
    IRepositoryManager repository,
    IValidator<Notification> validator,
    IMapper mapper,
    IHangfireService hangfireService,
    IMediator mediator) 
    : IRequestHandler<UpdateNotificationCommand>
{
    public async Task<Unit> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var taskId = request.NotificationDto.TaskId.ToString();
        var notifications = await repository.Notification.FindByCondition(
            c => c.TaskId.ToString() == taskId, cancellationToken);
        var notificationEntity = notifications.FirstOrDefault();

        if (notificationEntity == null)
        {
            await mediator.Send(new CreateNotificationCommand { NotificationDto = request.NotificationDto }, cancellationToken);
            return Unit.Value;
        }

        DateTime oldDeadline = notificationEntity.Deadline;
        
        mapper.Map(request, notificationEntity);

        var validationResult = await validator.ValidateAsync(notificationEntity, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors.ToString());
        }
        
        if (!oldDeadline.Equals(notificationEntity.Deadline))
        {
            notificationEntity.Deadline = DateTime.SpecifyKind(notificationEntity.Deadline, DateTimeKind.Local);
            notificationEntity.Deadline = TimeZoneInfo.ConvertTimeToUtc(notificationEntity.Deadline);

            hangfireService.DeleteNotificationInHangfire(notificationEntity.HangfireJobId);
            hangfireService.ScheduleNotificationInHangfire(notificationEntity, cancellationToken);
        }
        
        await repository.Notification.UpdateNotificationByTaskId(notificationEntity, cancellationToken);
        
        return Unit.Value;
    }
}