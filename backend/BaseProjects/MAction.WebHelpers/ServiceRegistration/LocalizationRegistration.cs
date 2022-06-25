using System.Globalization;
using MAction.BaseClasses.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace MAction.WebHelpers.ServiceRegistration;

public static class LocalizationRegistration
{
    public static void ConfigureLocalization(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var defaultLanguage = string.Empty;
            foreach (var field in typeof(LanguageEnum).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DefaultLanguage)) is not DefaultLanguage attribute)
                    continue;


                defaultLanguage = field.Name.ToLower();
                break;
            }

            options.DefaultRequestCulture = new RequestCulture(defaultLanguage);


            List<CultureInfo> language = Enum.GetNames(typeof(LanguageEnum)).Select(name => new CultureInfo(name))
                .ToList();
            CultureInfo[] cultures = language.ToArray();


            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
        });
    }

    public static void SetLanguageMiddleWare(this IApplicationBuilder app)
    {
        app.UseRequestLocalization();
    }
}