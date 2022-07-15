using MAction.AspNetIdentity.Base;
using MAction.BaseClasses;
using MAction.BaseServices.Configure;
using MAction.SampleOnion.Service.Company;
using MAction.SampleOnion.Service.MyProfile;
using MAction.SampleOnion.Service.ViewModel.Input;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MAction.SampleOnion.Service.Category;
using MAction.SipOnline.Service;

namespace MAction.SampleOnion.Service.Configure
{
    public class ServiceRegistration
    {
        public static IMvcBuilder AddFluentValidation(IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddFluentValidation(cfg =>
                cfg.RegisterValidatorsFromAssemblyContaining<SaleCompanyValidator>());
        }
        public static void AddConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            //We Only Need when model not inherit from BaseDTO
            //services.AddAutoMapper(typeof(SaleCompanyInputModel));
            var assembly = typeof(CategoryInputModel).Assembly;

            ConfigureAutoMapper.ConfigureAutoMapperFromServiceAndAssembly(services, assembly);

            PolicyLoader.AddPolices(typeof(DashboardPolicies).Assembly);

            services.AddScoped<IServiceDependencyProvider, ServiceDependencyProvider>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICompanyServiceWithExpression, CompanyServiceWithExpression>();
            services.AddScoped<IMyProfileService, MyProfileService>();
        }

    }
}
