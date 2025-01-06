using AutoMapper;
using eAppointmentServer.Application.Features.Users.UpdateUser;
using eAppointmentServer.Domain.Entities;
using eAppointmentServer.Domain.Repositories;
using GenericRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TS.Result;

internal sealed class UpdateUserCommandHandler(
    UserManager<AppUser> userManager,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<UpdateUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByIdAsync(request.Id.ToString());    // İlk olarak, güncellenmek istenen kullanıcının sistemde var olup olmadığı kontrol edilir. Eğer kullanıcı bulunamazsa "User not found" (Kullanıcı bulunamadı) hatası döner.
        if (user is null)
        {
            return Result<string>.Failure("User not found");
        }

        if (user.Email != request.Email)    // Kullanıcının e-posta adresi ya da kullanıcı adı değişmişse, veritabanında bu e-posta veya kullanıcı adına sahip başka bir kullanıcı olup olmadığı kontrol edilir. Eğer aynı e-posta veya kullanıcı adı başka bir kullanıcıda varsa, ilgili hata mesajı döner.
        {
            if (await userManager.Users.AnyAsync(p => p.Email == request.Email))
            {
                return Result<string>.Failure("Email already exists");
            }
        }

        if (user.UserName != request.UserName)
        {
            if (await userManager.Users.AnyAsync(p => p.UserName == request.UserName))
            {
                return Result<string>.Failure("User name already exists");
            }
        }

        mapper.Map(request, user);    // Eğer e-posta ve kullanıcı adı kontrolünden geçerse, AutoMapper kullanılarak request nesnesindeki yeni bilgiler mevcut kullanıcıya (user) kopyalanır. Daha sonra userManager.UpdateAsync ile bu değişiklikler veritabanına kaydedilir. Eğer güncelleme sırasında bir hata oluşursa, hata mesajları döner.
        IdentityResult result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result<string>.Failure(result.Errors.Select(s => s.Description).ToList());
        }


        if (request.RoleIds.Any())    // Eğer kullanıcıya atanacak roller varsa, önce kullanıcının mevcut rollerini (AppUserRole) veritabanından çekilir. Daha sonra eski roller silinir (userRoleRepository.DeleteRange). Yeni roller ise, request.RoleIds içinde belirtilen rollerle oluşturulur ve kullanıcıya atanır. Bu roller userRoleRepository.AddRangeAsync ile veritabanına eklenir ve tüm değişiklikler unitOfWork.SaveChangesAsync ile kaydedilir.
        {
            List<AppUserRole> userRoles = await userRoleRepository.Where(p => p.UserId == user.Id).ToListAsync();
            userRoleRepository.DeleteRange(userRoles);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            userRoles = new();

            foreach (var roleId in request.RoleIds)
            {
                AppUserRole userRole = new()
                {
                    RoleId = roleId,
                    UserId = user.Id
                };
                userRoles.Add(userRole);
            }

            await userRoleRepository.AddRangeAsync(userRoles, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return "User update is successful";
    }
}