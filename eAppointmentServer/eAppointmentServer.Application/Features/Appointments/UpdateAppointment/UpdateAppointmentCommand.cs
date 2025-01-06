using MediatR;
using TS.Result;

namespace eAppointmentServer.Application.Features.Appointments.UpdateAppointment
{
    public sealed record UpdateAppointmentCommand(
        Guid Id,
        string startDate,
        string endDate) : IRequest<Result<string>>;
}
