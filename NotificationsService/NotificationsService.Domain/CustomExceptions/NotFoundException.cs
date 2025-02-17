namespace NotificationsService.Domain.CustomExceptions;

public class NotFoundException(string message) : ApplicationException(message);
