namespace TasksService.Domain.CustomExceptions;

public class AlreadyExistsException(string message) : Exception(message);
