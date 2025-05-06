using Application.Contracts.Grpc;
using Grpc.Net.Client;
using GrpcService;

namespace TasksService.Infrastructure.Grpc;

public class GrpcUserService : IUserGrpcService
{
    public async Task<string> GetUserEmailAsync(string userId)
    {
        var channel = GrpcChannel.ForAddress("http://grpc-service:5220"); 
        var client = new UserService.UserServiceClient(channel);

        var request = new GetUserEmailRequest { UserId = userId };
        var response = await client.GetUserEmailAsync(request);
        
        return response.Email;
    }
}