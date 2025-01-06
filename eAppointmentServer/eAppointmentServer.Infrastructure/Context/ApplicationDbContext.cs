using eAppointmentServer.Domain.Entities;
using GenericRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace eAppointmentServer.Infrastructure.Context
{
    internal sealed class ApplicationDbContext : IdentityDbContext<AppUser, AppRole,Guid, IdentityUserClaim<Guid>, AppUserRole,IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,IdentityUserToken<Guid>>, IUnitOfWork // internal olarak bırakmamızın sebebi bu Class'ı başka hiçbir katman kullanamayacak,  IdentityDbContext bu özellik ile ilgili katmanın referanslarına erişim sağlayabiliriz.
        // IUnitOfWork  -->> işlemleri bir arada yönetme amacını gösterir. Yani bu sınıf, birden fazla veritabanı işlemini tek bir "birim" olarak ele alır. Bu sayede, tüm işlemler başarılıysa veritabanına kaydedilir, başarısız olursa hepsi geri alınır (transactional consistency).
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) // Bu constructor ile veritabanı bağlantı seçenekleri dışarıdan alınabilir. Örneğin Program.cs dosyasında options ile yapılandırılabilir. DbContextOptions kısmı DependencyInjection kısmında doldurulacak.
        {
            
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Ignore<IdentityUserClaim<Guid>>(); // Identity kütüphanesindeki kullanılmayan classları ignore ettik.
            builder.Ignore<IdentityRoleClaim<Guid>>();
            builder.Ignore<IdentityUserLogin<Guid>>();
            builder.Ignore<IdentityUserToken<Guid>>();

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // IEntityTypeConfiguration interfacesini uygulayabilmek için bu interfacenin implement edildiği katmanı burada belirtmek zorundayız.
        }
    }

}
