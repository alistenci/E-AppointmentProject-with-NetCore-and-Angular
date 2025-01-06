using Microsoft.Extensions.DependencyInjection;

namespace eAppointmentServer.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services) {  // extension metod
            services.AddAutoMapper(typeof(DependencyInjection));
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); // Bulunduğu Application katmanının bilgisini, assemblysini verdik.
            });
            return services;
        }
    }
}
