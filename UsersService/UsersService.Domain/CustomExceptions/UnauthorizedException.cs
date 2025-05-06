namespace UsersService.Domain.CustomExceptions;

public class UnauthorizedException(string message) : Exception(message);
