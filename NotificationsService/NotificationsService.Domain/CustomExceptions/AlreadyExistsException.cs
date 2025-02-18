namespace NotificationsService.Domain.CustomExceptions;

public class AlreadyExistsException(string message) : Exception(message);
