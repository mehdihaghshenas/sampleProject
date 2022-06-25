using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using MAction.SampleOnion.Service.Configure;
using Infrastructure.EmailService;
using MAction.SampleOnion.Infrastructure.EmailService;
using MAction.AspNetIdentity.Base;

//using AutoMapperBuilder.Extensions.DependencyInjection;

//using PR.Service.HRModels.LoanPaymentModel;

namespace MAction.SampleOnion.Infrastructure.Configure
{
    public class ServiceRegistration
    {
        public static void AddConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            // Config Email Server Setting
            services.Configure<EmailServerSetting>(
                configuration.GetSection("EmailServerSettings")
            );
            services.AddScoped<IEmailSender, EmailSender>();
        }

    }
}