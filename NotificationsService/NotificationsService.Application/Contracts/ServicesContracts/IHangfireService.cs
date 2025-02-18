using NotificationsService.Domain.Models;

namespace NotificationsService.Application.Contracts.ServicesContracts;

public interface IHangfireService
{
    public string ScheduleNotificationInHangfire(Notification notification, CancellationToken cancellationToken);
    
    public void DeleteNotificationInHangfire(string hangfireJobId);
}