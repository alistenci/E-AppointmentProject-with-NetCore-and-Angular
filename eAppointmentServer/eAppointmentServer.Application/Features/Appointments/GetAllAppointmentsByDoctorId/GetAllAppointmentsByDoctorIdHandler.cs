using eAppointmentServer.Application.Features.Appointments.GetAllAppointments;
using eAppointmentServer.Domain.Entities;
using eAppointmentServer.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TS.Result;

internal sealed class GetAllAppointmentsByDoctorIdHandler(IAppointmentRepository appointmentRepository) : IRequestHandler<GetAllAppointmentsByDoctorIdQuery, Result<List<GetAllAppointmentsByDoctorIdResponse>>>
{
    public async Task<Result<List<GetAllAppointmentsByDoctorIdResponse>>> Handle(GetAllAppointmentsByDoctorIdQuery request, CancellationToken cancellationToken)
    {
        List<Appointment> appointments = await appointmentRepository.Where(p => p.DoctorId == request.DoctorId).Include(p => p.Patient).ToListAsync(cancellationToken);

        List<GetAllAppointmentsByDoctorIdResponse> response = appointments.Select(s => new GetAllAppointmentsByDoctorIdResponse(
            s.Id,
            s.StartDate,
            s.EndDate,
            s.Patient!.FullName,
            s.Patient
            )).ToList();

        return response;

    }
}

