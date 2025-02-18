namespace NotificationsService.Application.Contracts.ServicesContracts;

public interface ISmtpService
{
    public Task SendEmailAsync(string to, string subject, string body);
}