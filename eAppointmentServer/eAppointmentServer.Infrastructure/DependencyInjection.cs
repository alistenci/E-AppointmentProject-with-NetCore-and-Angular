using eAppointmentServer.Domain.Entities;
using eAppointmentServer.Infrastructure.Context;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace eAppointmentServer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseNpgsql(configuration.GetConnectionString("PostgreServer"));
                    });
            services.AddIdentity<AppUser, AppRole>(action =>
            {
                action.Password.RequiredLength = 1;
                action.Password.RequireUppercase = false;
                action.Password.RequireLowercase = false;
                action.Password.RequireNonAlphanumeric = false;
                action.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>()); // Bu kod, IUnitOfWork talep eden yerlere, aslında bir ApplicationDbContext örneğini enjekte etmeyi sağlar. Bu sayede, veritabanı bağlamı olarak kullanılan ApplicationDbContext, aynı zamanda bir Unit of Work olarak davranır ve birden fazla veritabanı işlemini yönetme sorumluluğunu taşır.

            services.Scan(action =>
            {
                action.FromAssemblies(typeof(DependencyInjection).Assembly). // Hangi katmanlardaki classlar için DI uygulanacaksa bunu belirt.Bu katmandakiler için uygulanacak bu yüzden typeof(DependencyInjection).Assembly bu kod ile sadece bu katmandaki DI'ı verdik.
                AddClasses(publicOnly: false). // publicOnly'i false yaparak internal ve private olanları da dahil ettik.
                UsingRegistrationStrategy(registrationStrategy: RegistrationStrategy.Skip). // Eğer bir class'a zaten DI uygulanmışsa bu class'ı atla.
                AsImplementedInterfaces(). // Bir sınıfın implement ettiği tüm arayüzleri otomatik olarak DI sistemine kaydeder.
                WithScopedLifetime(); // Scoped yaşam döngüsünü kullan
            }); // İsimleri aynı olduğu sürece IProductRepository, ProductRepository gibi bunların DI'sını otomatik olarak yap.


            //services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            //services.AddScoped<IDoctorRepository, DoctorRepository>();
            //services.AddScoped<IPatientRepository, PatientRepository>();

            //services.AddScoped<IJwtProvider, JwtProvider>();

            return services;
        }
    }
}
