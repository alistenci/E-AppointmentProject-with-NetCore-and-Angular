using MediatR;
using TS.Result;

namespace eAppointmentServer.Application.Features.Users.CreateUser
{
    public sealed record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string UserName,
        List<Guid> RoleIds) : IRequest<Result<string>>;
}
