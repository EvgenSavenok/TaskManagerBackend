namespace UsersService.Domain.CustomExceptions;

public class AlreadyExistsException(string message) : Exception(message);
