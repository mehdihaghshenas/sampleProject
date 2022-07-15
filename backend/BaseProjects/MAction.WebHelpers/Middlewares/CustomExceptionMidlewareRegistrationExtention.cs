using Microsoft.AspNetCore.Builder;

namespace MAction.WebHelpers.Middlewares;

public static class CustomExceptionMiddlewareRegistrationExtension
{
    public static void CustomExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CustomExceptionMiddleware>();
    }
}
