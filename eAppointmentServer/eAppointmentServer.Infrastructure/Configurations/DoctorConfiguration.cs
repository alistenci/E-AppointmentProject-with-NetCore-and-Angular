﻿using eAppointmentServer.Domain.Entities;
using eAppointmentServer.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eAppointmentServer.Infrastructure.Configurations
{
    internal sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.Property(p => p.FirstName).HasColumnType("varchar(50)");
            builder.Property(p => p.LastName).HasColumnType("varchar(50)");
            //builder.HasIndex(x => x.FirstName).IsUnique(); // Uniq isim tanımlanması için kullandık.

            builder.Property(p => p.Department).HasConversion(v => v.Value, v => DepartmentEnum.FromValue(v)).HasColumnName("Department");
        }
    }
}
