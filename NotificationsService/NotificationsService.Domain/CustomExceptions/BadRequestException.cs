namespace NotificationsService.Domain.CustomExceptions;

public class BadRequestException(string message) : Exception(message);
