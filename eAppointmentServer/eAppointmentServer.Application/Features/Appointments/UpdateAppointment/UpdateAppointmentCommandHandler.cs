using eAppointmentServer.Application.Features.Appointments.UpdateAppointment;
using eAppointmentServer.Domain.Entities;
using eAppointmentServer.Domain.Repositories;
using GenericRepository;
using MediatR;
using TS.Result;

internal sealed class UpdateAppointmentCommandHandler(
    IAppointmentRepository appointmentRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateAppointmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        Appointment? appointment = await appointmentRepository.GetByExpressionWithTrackingAsync(p => p.Id == request.Id, cancellationToken);

        DateTime startDate = Convert.ToDateTime(request.startDate).ToUniversalTime();
        DateTime endDate = Convert.ToDateTime(request.endDate).ToUniversalTime();

        if (appointment is null)
        {
            return Result<string>.Failure("Appointment not found");
        }

        // Aynı saatte tek randevu olsun


        bool isAppointmentDateNotAvailable = await appointmentRepository
            .AnyAsync(p => p.DoctorId == appointment.DoctorId &&
             ((p.StartDate < endDate && p.StartDate >= startDate) || // Mevcut randevunun bitişi, diğer randevunun başlangıcıyla çakışıyor
             (p.EndDate > startDate && p.EndDate <= endDate) || // Mevcut randevunun başlangıcı, diğer randevunun bitişiyle çakışıyor
             (p.StartDate >= startDate && p.EndDate <= endDate) || // Mevcut randevu, diğer randevu içinde tamamen
             (p.StartDate <= startDate && p.EndDate >= endDate)), // Mevcut randevu, diğer randevuyu tamamen kapsıyor
             cancellationToken);

        if (isAppointmentDateNotAvailable)
        {
            return Result<string>.Failure("Appointment date is not available ");
        }


        // Aynı saatte tek randevu olsun

        appointment.StartDate = Convert.ToDateTime(request.startDate).ToUniversalTime();
        appointment.EndDate = Convert.ToDateTime(request.endDate).ToUniversalTime();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Appointment update is successful";
    }
}
