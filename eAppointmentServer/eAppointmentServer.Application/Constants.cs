using eAppointmentServer.Domain.Entities;

namespace eAppointmentServer.Application
{
    public static class Constants
    {
        public static List<AppRole> GetRoles()
        {
            List<string> roles = new()
            {
                "Admin",
                "Doctor",
                "Patient",
                "Personel"
            };
            return roles.Select(s => new AppRole()
            {
                Name = s
            }).ToList();
        }
    }
}