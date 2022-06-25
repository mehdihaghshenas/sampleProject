using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using MAction.BaseMongoRepository;
using MAction.AspNetIdentity.Base;
using MAction.AspNetIdentity.Mongo.Domain;
using Microsoft.Extensions.DependencyInjection;
using MAction.AspNetIdentity.Mongo.Service;
using MAction.AspNetIdentity.Base.Repository;
using MAction.AspNetIdentity.Mongo.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MAction.AspNetIdentity.Mongo;
public class ServiceRegistration
{
    public static void AddConfigureService(IServiceCollection services, string connectionString, IMongoDependencyProvider mongoDependencyProvider, JwtSettings jwtSettings, Type userEmailSender)
    {
        var connectionBuilder = new MongoUrlBuilder(connectionString)
        {
            DatabaseName = mongoDependencyProvider.DatabaseName
        };

        if (!typeof(IUserEmailSender).IsAssignableFrom(userEmailSender))
            throw new Exception("userEmailSender should be assignble from IUserEmailSender");

        services.AddScoped(typeof(IUserEmailSender), userEmailSender);
        services.AddScoped<IJWTService, JwtService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddSingleton<IVerificationCodeSingletonRepository, VerificationCodeSingletonRepository>();

        services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole, ObjectId>(identity =>
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
        },
        mongo =>
        {
            mongo.ConnectionString = connectionBuilder.ToString();//mongodb://root:A1b2c3d4@localhost:27017/?authSource=admin&readPreference=primary&ssl=false
            // other options
        }
        );

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

        PolicyLoader.AddPolices(typeof(SecurityPolicies).Assembly);
        services.AddAuthorization(options =>
        {
            PolicyLoader.RegisterPolicies(options);
        });

    }

    public static IApplicationBuilder Configure(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseRouting();
        app.UseMiddleware<JwtMiddleware>();
        return app.UseAuthorization();
    }
}