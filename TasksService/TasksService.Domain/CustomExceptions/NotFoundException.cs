namespace TasksService.Domain.CustomExceptions;

public class NotFoundException(string message) : ApplicationException(message);
