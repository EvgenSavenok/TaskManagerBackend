using AutoMapper;
using Hangfire;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Application.UseCases.Queries.NotificationQueries.GetNotificationById;
using NotificationsService.Domain.Enums;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.EmailService;

public class HangfireService(
    ISmtpService smtpService, 
    IMediator mediator,
    IRepositoryManager repositoryManager,
    IMapper mapper)
    : IHangfireService
{
    public string ScheduleNotificationInHangfire(
        Notification notification, 
        CancellationToken cancellationToken)
    {
        DateTime nowUtc = DateTime.UtcNow; 
        TimeSpan delay = notification.ReminderTime - nowUtc;
        
        var jobId = notification.HangfireJobId = BackgroundJob.Schedule(
            () => SendNotificationEmail(notification.Id, cancellationToken), delay);
        return jobId;
    }
    
    public async Task SendNotificationEmail(
        Guid notificationId, 
        CancellationToken cancellationToken)
    {
        var query = new GetNotificationQuery
        {
            NotificationId = notificationId
        };
        var notificationDto = await mediator.Send(query, cancellationToken);
        
        TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(notificationDto.UserTimeZone); 
        DateTime userDeadline = TimeZoneInfo.ConvertTimeFromUtc(notificationDto.Deadline, userTimeZone);
        
        string emailBody = 
            $"<h1>{notificationDto.Title}</h1><p>Напоминание о задаче. Дедлайн: {userDeadline}</p>";
        
        await smtpService.SendEmailAsync(
            notificationDto.UserEmail, 
            "Напоминание о задаче", 
            emailBody);
        
        var notificationEntity = new Notification();
        
        mapper.Map(notificationDto, notificationEntity);
        
        notificationEntity.Status = Status.Sleep;
        
        await repositoryManager.Notification.UpdateNotificationByTaskId(
            notificationEntity, 
            cancellationToken);
    }

    public void DeleteNotificationInHangfire(string hangfireJobId)
    {
        BackgroundJob.Delete(hangfireJobId);
    }
}