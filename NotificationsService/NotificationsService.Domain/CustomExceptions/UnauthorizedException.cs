namespace NotificationsService.Domain.CustomExceptions;

public class UnauthorizedException(string message) : ApplicationException(message);
