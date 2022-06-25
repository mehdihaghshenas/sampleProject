using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using MAction.BaseClasses.Exceptions;

namespace MAction.WebHelpers.Middlewares;

public class CustomExceptionMidleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionMidleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomApplicationException ex)
        {
            await GenerateResponse(context, ex.HttpStatusCode, new Dictionary<string, string>()
            {
                ["title"] = ex.Message,
                ["StackTrace"] = ex.StackTrace ?? ""
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            await GenerateResponse(context, HttpStatusCode.Unauthorized, new Dictionary<string, string>()
            {
                ["title"] = ex.Message,
                ["StackTrace"] = ex.StackTrace ?? ""
            });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("IDX12729"))
            {
                await GenerateResponse(context, HttpStatusCode.Unauthorized, new Dictionary<string, string>()
                {
                    ["title"] = ex.Message,
                    ["StackTrace"] = ex.StackTrace ?? ""
                });
            }
            else
                await GenerateResponse(context, HttpStatusCode.InternalServerError, new Dictionary<string, string>()
                {
                    ["title"] = ex.Message,
                    ["StackTrace"] = ex.StackTrace ?? ""
                });
        }
    }

    private static async Task GenerateResponse(HttpContext context, HttpStatusCode statusCode, Dictionary<string, string> messages)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(messages));
    }
}