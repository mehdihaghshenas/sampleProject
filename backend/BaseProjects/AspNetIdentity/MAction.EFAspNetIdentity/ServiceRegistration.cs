
using Microsoft.Extensions.DependencyInjection;

using System.Text;
using MAction.AspNetIdentity.EFCore;
using MAction.AspNetIdentity.Base;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using MAction.AspNetIdentity.EFCore.Repository;
using Microsoft.AspNetCore.Identity;
using MAction.AspNetIdentity.Base.Entities;
using MAction.AspNetIdentity.Base.Services;
using MAction.AspNetIdentity.Base.Repository;

namespace MAction.BaseClasses;
public class ServiceRegistration
{
    public static void AddConfigureService<TContext, TUser, TRole, TKey>(IServiceCollection services, string connectionString, JwtSettings jwtSettings, Type userEmailSender)
        where TContext : IdentityContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>, IUser, IBaseEntity, new()
        where TRole : IdentityRole<TKey>, IRole, IBaseEntity, new()
        where TKey : IEquatable<TKey>
    {
        if (!typeof(IUserEmailSender).IsAssignableFrom(userEmailSender))
            throw new Exception("userEmailSender should be assignble from IUserEmailSender");

        services.AddScoped(typeof(IUserEmailSender), userEmailSender);
        services.AddScoped<IJWTService, JwtService<TUser, TRole, TKey>>();
        services.AddScoped<IUserService, UserService<TUser, TRole, TKey>>();
        services.AddScoped<IRoleService, RoleService<TRole, TKey>>();
        services.AddScoped<IUserRepository<TUser, TRole, TKey>, UserRepository<TUser, TRole, TKey>>();
        services.AddScoped<IRoleRepository<TRole, TKey>, RoleRepository<TUser, TRole, TKey>>();

        services.AddSingleton<IVerificationCodeSingletonRepository, VerificationCodeSingletonRepository>();

        services.AddIdentity<TUser,TRole>(identity =>
        {
            identity.Password.RequiredLength = jwtSettings.RequiredLength;
            identity.Password.RequireDigit = jwtSettings.PasswordRequireDigit;
            identity.Password.RequiredLength = jwtSettings.PasswordRequiredLength;
            identity.Password.RequireNonAlphanumeric = jwtSettings.PasswordRequireNonAlphanumic;
            identity.Password.RequireUppercase = jwtSettings.PasswordRequireUppercase;
            identity.Password.RequireLowercase = jwtSettings.PasswordRequireLowercase;
            identity.User.RequireUniqueEmail = jwtSettings.RequireUniqueEmail;
            identity.SignIn.RequireConfirmedEmail = false;
            identity.SignIn.RequireConfirmedPhoneNumber = false;
            identity.Lockout.MaxFailedAccessAttempts = 5;
            identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            identity.Lockout.AllowedForNewUsers = false;
            // other options
        }
        ).AddEntityFrameworkStores<TContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>

               {
                   var secretKey = Encoding.UTF8.GetBytes(jwtSettings.IdentitySecretKey);

                   var validationParameters = new TokenValidationParameters
                   {

                       ClockSkew = TimeSpan.Zero,
                       RequireSignedTokens = true,

                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                       RequireExpirationTime = true,
                       ValidateLifetime = true,

                       ValidateAudience = true,
                       ValidAudience = jwtSettings.Audience,

                       ValidateIssuer = true,
                       ValidIssuer = jwtSettings.Issuer

                   };

                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;

                   options.TokenValidationParameters = validationParameters;
               });

        services.AddAuthorization(options =>
        {
            PolicyLoader.RegisterPolicies(options);
        });

        PolicyLoader.AddPolices(typeof(SecurityPolicies).Assembly);


    }
    public static void Configure(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<JwtMiddleware>();


    }
}