using Hangfire;
using MediatR;
using NotificationsService.Application.Contracts.RepositoryContracts;
using NotificationsService.Application.Contracts.ServicesContracts;
using NotificationsService.Application.UseCases.Queries.NotificationQueries.GetNotificationById;
using NotificationsService.Domain.Models;

namespace NotificationsService.Application.EmailService;

public class HangfireService(
    ISmtpService smtpService, 
    IMediator mediator,
    IRepositoryManager repositoryManager)
    : IHangfireService
{
    public string ScheduleNotificationInHangfire(Notification notification, CancellationToken cancellationToken)
    {
        DateTime nowUtc = DateTime.UtcNow; 
        TimeSpan delay = notification.ReminderTime - nowUtc;
        
        var jobId = notification.HangfireJobId = BackgroundJob.Schedule(
            () => SendNotificationEmail(notification.Id, cancellationToken), delay);
        return jobId;
    }

    /// <summary>
    /// 
    /// TODO
    /// Need to get real user email from the broker
    /// 
    /// Questions
    /// Is it okay to use mediator in the Application?
    /// 
    /// </summary>
    public async Task SendNotificationEmail(Guid notificationId, CancellationToken cancellationToken)
    {
        var query = new GetNotificationQuery
        {
            NotificationId = notificationId
        };
        var notification = await mediator.Send(query, cancellationToken);
        
        TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(notification.UserTimeZone); 
        DateTime userDeadline = TimeZoneInfo.ConvertTimeFromUtc(notification.Deadline, userTimeZone);
        
        string emailBody = 
            $"<h1>{notification.Title}</h1><p>Напоминание о задаче. Дедлайн: {userDeadline}</p>";
        
        await smtpService.SendEmailAsync(
            notification.UserEmail, 
            "Напоминание о задаче", 
            emailBody);
        
        await repositoryManager.Notification.Delete(n => n.Id == notificationId, cancellationToken);
    }

    public void DeleteNotificationInHangfire(string hangfireJobId)
    {
        BackgroundJob.Delete(hangfireJobId);
    }
}