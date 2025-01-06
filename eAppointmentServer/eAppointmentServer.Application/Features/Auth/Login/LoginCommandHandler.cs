using eAppointmentServer.Application.Services;
using eAppointmentServer.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TS.Result;

namespace eAppointmentServer.Application.Features.Auth.Login
{
    internal sealed class LoginCommandHandler(UserManager<AppUser> userManager,IJwtProvider jwtProvider) : 
        IRequestHandler<LoginCommand, Result<LoginCommandResponse>>

    //UserManager, ASP.NET Core Identity sisteminde kullanıcıları yönetmek için kullanılan bir sınıftır. AppUser, kullanıcı bilgilerini temsil eden bir kullanıcı sınıfıdır.
    {
        public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        //Task sınıfı, asenkron bir işlemin sonucunu temsil eder. Result<LoginCommandResponse> türü ise bu asenkron işlemin sonucunu kapsar.
        {
            AppUser? appUser = await userManager.Users.FirstOrDefaultAsync(p =>
              p.UserName == request.usernameOrEmail ||
              p.Email == request.usernameOrEmail, cancellationToken);

            if(appUser is null)
            {
                return Result<LoginCommandResponse>.Failure("User not found");
            }
            bool isPasswordCorrect = await userManager.CheckPasswordAsync(appUser, request.password);
            if (!isPasswordCorrect)
            {
                return Result<LoginCommandResponse>.Failure("Password is wrong");
            }

            string token = await jwtProvider.CreateTokenAsync(appUser);
            return Result<LoginCommandResponse>.Succeed(new LoginCommandResponse(token));
        }
    }
}
