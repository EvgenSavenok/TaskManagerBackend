namespace TasksService.Domain.CustomExceptions;

public class BadRequestException(string message) : ApplicationException(message);
