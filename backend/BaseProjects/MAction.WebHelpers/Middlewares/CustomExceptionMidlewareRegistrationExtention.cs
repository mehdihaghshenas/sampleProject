using Microsoft.AspNetCore.Builder;

namespace MAction.WebHelpers.Middlewares;

public static class CustomExceptionMidlewareRegistrationExtention
{
    public static void CustomExceptionMidleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CustomExceptionMidleware>();
    }
}
