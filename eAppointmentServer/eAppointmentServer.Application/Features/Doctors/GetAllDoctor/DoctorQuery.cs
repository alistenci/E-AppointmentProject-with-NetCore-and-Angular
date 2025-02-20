﻿using eAppointmentServer.Domain.Entities;
using MediatR;
using TS.Result;

namespace eAppointmentServer.Application.Features.Doctors.GetAllDoctor
{
    public sealed record DoctorQuery() : IRequest<Result<List<Doctor>>>;
}
