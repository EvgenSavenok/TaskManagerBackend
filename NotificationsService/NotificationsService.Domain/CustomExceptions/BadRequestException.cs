namespace NotificationsService.Domain.CustomExceptions;

public class BadRequestException(string message) : ApplicationException(message);
