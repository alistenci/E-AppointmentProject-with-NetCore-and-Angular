using AutoMapper;
using eAppointmentServer.Application.Features.Users.CreateUser;
using eAppointmentServer.Domain.Entities;
using eAppointmentServer.Domain.Repositories;
using GenericRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TS.Result;

internal sealed class CreatUserCommandHandler(
    UserManager<AppUser> userManager,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if(await userManager.Users.AnyAsync(p => p.Email == request.Email))    // Veritabanında e-posta adresi ya da kullanıcı adı zaten mevcut mu diye kontrol yapılır. Eğer aynı e-posta ya da kullanıcı adı mevcutsa, hata döner ve işlem sonlanır.
        {
            return Result<string>.Failure("Email already exists");
        }

        if (await userManager.Users.AnyAsync(p => p.UserName == request.UserName))
        {
            return Result<string>.Failure("Username already exists");
        }

        AppUser user = mapper.Map<AppUser>(request);    // Eğer e-posta ve kullanıcı adı mevcut değilse, yeni bir AppUser nesnesi oluşturulur. mapper.Map<AppUser>(request) ifadesi, request nesnesindeki bilgileri kullanarak yeni bir AppUser nesnesi oluşturur. Bu işlemde AutoMapper kütüphanesi kullanılır. Daha sonra userManager.CreateAsync ile bu kullanıcı veritabanına eklenir.
        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Result<string>.Failure(result.Errors.Select(s => s.Description).ToList());    // Kullanıcı oluşturulurken bir hata meydana gelirse, hata mesajları döner. Hatalar result.Errors.Select(s => s.Description).ToList() ile toplanır.
        }

        if (request.RoleIds.Any())    // Kullanıcıya atanacak roller varsa (yani RoleIds listesi doluysa), her bir rol için bir AppUserRole nesnesi oluşturulur. Her AppUserRole nesnesi, kullanıcının ID’si ve rol ID’sini içerir. Bu rol bilgileri veritabanına eklenir ve tüm değişiklikler unitOfWork.SaveChangesAsync ile kaydedilir.
        {
            List<AppUserRole> userRoles = new();
            foreach (var roleId in request.RoleIds)
            {
                AppUserRole userRole = new()
                {
                    RoleId = roleId,
                    UserId = user.Id
                };
                userRoles.Add(userRole);

                await userRoleRepository.AddRangeAsync(userRoles, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return "User create is successful";
    }
}
