using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MAction.BaseClasses;
using Microsoft.Extensions.DependencyInjection;

namespace MAction.BaseServices.Configure;

public class CustomMappingProfile : Profile
{
    public CustomMappingProfile(Assembly baseAssembly)
    {
        var types = baseAssembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract &&
                                                       typeof(IBaseDto).IsAssignableFrom(x));

        foreach (var type in types)
        {
            var ins = (IBaseDto)Activator.CreateInstance(type);
            ins?.ConfigureMapping(this);
        }
    }
}

public class ConfigureAutoMapper
{
    public static void ConfigureAutoMapperFromServiceAndAssembly(IServiceCollection services, Assembly assembly)
    {
        services.AddAutoMapper(c => { c.AddProfile(new CustomMappingProfile(assembly)); }, assembly);
    }
}