namespace Application.CustomExceptions;

public class AlreadyExistsException(string message) : Exception(message);
