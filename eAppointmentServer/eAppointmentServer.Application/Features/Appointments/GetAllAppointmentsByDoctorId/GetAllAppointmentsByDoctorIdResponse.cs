using eAppointmentServer.Domain.Entities;

public sealed record GetAllAppointmentsByDoctorIdResponse(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    string title,
    Patient Patient);

