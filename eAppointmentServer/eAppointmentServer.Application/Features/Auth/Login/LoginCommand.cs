using MediatR;
using TS.Result;

namespace eAppointmentServer.Application.Features.Auth.Login
{
    public sealed record LoginCommand(string usernameOrEmail, string password) : IRequest<Result<LoginCommandResponse>>;
}
