using Grpc.Core;
using GrpcService;
using MediatR;
using UsersService.Application.UseCases.Queries.UserQueries.GetUserById;

namespace UsersService.Grpc.Services;

public class UserGrpcService(IMediator mediator) : UserService.UserServiceBase
{
    public override async Task<GetUserEmailResponse> GetUserEmail(
        GetUserEmailRequest request, 
        ServerCallContext context)
    {
        var query = new GetUserByIdQuery
        {
            UserId = request.UserId
        };
        var user = await mediator.Send(query);
        
        return new GetUserEmailResponse { Email = user.Email };
    }
}