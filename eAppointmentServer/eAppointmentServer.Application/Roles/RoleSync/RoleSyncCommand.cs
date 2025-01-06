using MediatR;
using TS.Result;

namespace eAppointmentServer.Application.Roles.RoleSync
{
    public sealed record RoleSyncCommand() : IRequest<Result<string>>;
}
