using MediatR;
using UsersService.Domain;

namespace UsersService.Application.UseCases.Queries.UserQueries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IEnumerable<User>>;