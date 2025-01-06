using eAppointmentServer.Application.Services;
using eAppointmentServer.Domain.Entities;
using eAppointmentServer.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace eAppointmentServer.Infrastructure.Services
{
    internal sealed class JwtProvider(IConfiguration configuration,
        IUserRoleRepository userRoleRepository,
        RoleManager<AppRole> roleManager) : IJwtProvider
    {
        public async Task<string> CreateTokenAsync(AppUser user)
        {
            List<AppUserRole> appUserRoles = await userRoleRepository.Where(p => p.UserId == user.Id).ToListAsync();

            List<AppRole> roles = new();
            foreach (var userRole in appUserRoles)
            {
                AppRole? role = await roleManager.Roles.Where(p => p.Id == userRole.RoleId).FirstOrDefaultAsync();
                if(role is not null)
                {
                    roles.Add(role);
                }
            }

            List<string?> stringRoles = roles.Select(s => s.Name).ToList();

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty), // eğer user.Email null gelirse değer olarak string.Empty gönder
                new Claim("UserName", user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, JsonSerializer.Serialize(stringRoles))
            };

            DateTime expires = DateTime.Now.AddDays(1); // 1 Gün

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:SecretKey").Value ?? ""));


            //SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(string.Join("-", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())));

            // Encoding.UTF8.GetBytes(...): Bu kısım, belirli bir metni (string) UTF-8 formatında bayt dizisine dönüştürür. Bayt dizisi, şifreleme algoritmaları tarafından kullanılacak bir anahtara dönüştürülür.

            // string.Join("-", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()): Bu kısım, üç adet rastgele GUID oluşturur ve bunları birleştirir. Her bir GUID, benzersiz bir tanımlayıcıdır ve güvenli anahtar üretimi için kullanılır. GUID'ler arasına "-" işareti eklenir.

            // SymmetricSecurityKey: Simetrik anahtar, aynı anahtarı kullanarak hem şifreleme hem de şifre çözme işlemlerini gerçekleştiren bir şifreleme türüdür. JWT imzalama işlemi için simetrik anahtar kullanıldığında, hem token'ı imzalamak hem de imzanın doğruluğunu kontrol etmek için aynı anahtar kullanılır.


            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha512);
            // Bu satır, JWT'nin imzalanmasında kullanılacak imzalama bilgilerini tanımlar. SigningCredentials, token'ın imzalanmasında hangi güvenlik anahtarının ve hangi algoritmanın kullanılacağını belirler.

            // securityKey: Yukarıda oluşturulan SymmetricSecurityKey nesnesidir. Bu anahtar, token'ın imzalanması için kullanılacak.

            // SecurityAlgorithms.HmacSha512: Bu parametre, imzalama işleminde hangi şifreleme algoritmasının kullanılacağını belirtir. Burada HMAC-SHA512 algoritması kullanılıyor. HMAC-SHA512, güvenli bir hash algoritmasıdır ve simetrik anahtar ile birlikte kullanıldığında güçlü bir imza oluşturur.


            JwtSecurityToken jwtSecurityToken = new(
                issuer: configuration.GetSection("Jwt:Issuer").Value, // uygulama kim tarafından oluşturuldu
                audience: configuration.GetSection("Jwt:Audience").Value, // uygulamayı kim kullanacak
                claims: claims,
                notBefore: DateTime.Now, // token ne zamandan sonra kullanılmaya başlanacak
                expires: expires, // token ne zaman sonlancak
                signingCredentials: signingCredentials // şifreleme türü
                );

            JwtSecurityTokenHandler handler = new();  // Bu nesne, JWT'leri oluşturmak, doğrulamak ve işlemek için kullanılır.

            string token = handler.WriteToken( jwtSecurityToken );  // Bu satır, oluşturulan JwtSecurityToken nesnesini bir string olarak JWT formatına dönüştürür. Yani, oluşturulan token'ın bir dizge (string) temsilini alır. Bu dize formatında token, daha sonra bir API'ye veya istemciye geri döndürülerek kullanılabilir.

            return token;
        }
    }
}
