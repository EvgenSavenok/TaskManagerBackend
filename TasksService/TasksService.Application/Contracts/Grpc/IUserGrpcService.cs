namespace Application.Contracts.Grpc;

public interface IUserGrpcService
{
    Task<string> GetUserEmailAsync(string userId);
}