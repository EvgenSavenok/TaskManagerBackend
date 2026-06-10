using MediatR;
using UsersService.Domain;

namespace UsersService.Application.UseCases.Queries.UserQueries.GetUserById;

public record GetUserByIdQuery : IRequest<User>
{
    public string UserId { get; set; } 
}