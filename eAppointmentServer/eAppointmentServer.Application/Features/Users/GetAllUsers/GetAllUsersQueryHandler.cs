using eAppointmentServer.Application.Features.Users.GetAllUsers;
using eAppointmentServer.Domain.Entities;
using eAppointmentServer.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TS.Result;

internal sealed class GetAllUsersQueryHandler(UserManager<AppUser> userManager, IUserRoleRepository userRoleRepository,
    RoleManager<AppRole> roleManager) : IRequestHandler<GetAllUsersQuery, Result<List<GetAllUsersQueryResponse>>>
{
    public async Task<Result<List<GetAllUsersQueryResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        List<AppUser> users = await userManager.Users.OrderBy(p => p.FirstName).ToListAsync(cancellationToken); // db'deki tüm kullanıcılar çekildi
        List<GetAllUsersQueryResponse> response = users.Select(s => new GetAllUsersQueryResponse()  //Her bir kullanıcı için GetAllUsersQueryResponse nesneleri oluşturuluyor ve bu nesneler, kullanıcı bilgilerini içeriyor (ID, Ad, Soyad, Kullanıcı Adı, E-posta).
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            FullName = s.FullName,
            UserName = s.UserName,
            Email = s.Email
        }).ToList();

        foreach (var item in response) //Bu döngüde, her bir kullanıcı için onların sahip olduğu roller veritabanından userRoleRepository üzerinden çekiliyor. Her kullanıcının rollerine ulaşmak için UserId ile eşleşen kayıtlar alınıyor.
        {
            List<AppUserRole> userRoles = await userRoleRepository.Where(p => p.UserId == item.Id).ToListAsync(cancellationToken);

            List<Guid> stringRoles = new();
            List<string?> stringRoleNames = new();

            foreach (var userRole in userRoles) // Bu aşamada, her kullanıcının rollerinin ID'si (RoleId) kullanılarak rollerin isimleri (Name) çekiliyor ve RoleIds listesine ekleniyor.
            {
                AppRole? role =
                   await roleManager
                   .Roles
                   .Where(p => p.Id == userRole.RoleId)
                   .FirstOrDefaultAsync(cancellationToken);

                if (role is not null)
                {
                    stringRoles.Add(role.Id);
                    stringRoleNames.Add(role.Name);
                }
            }

            item.RoleIds = stringRoles;
            item.RoleNames = stringRoleNames;
        }

        return response;
    }
}