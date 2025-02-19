namespace TasksService.Domain.CustomExceptions;

public class NotFoundException(string message) : Exception(message);
