using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using MAction.BaseClasses.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MAction.WebHelpers.Middlewares;

public class CustomExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    public CustomExceptionMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadRequestException ex)
        {
            var exceptionDictionary = new Dictionary<string, object>()
            {
                ["title"] = ex.Message,
                ["errors"] = ex.Errors
            };

            if (_environment.IsDevelopment())
            {
                exceptionDictionary["StackTrace"] = ex.StackTrace ?? "";
            }

            await GenerateResponse(context, ex.HttpStatusCode, exceptionDictionary);
        }
        catch (CustomApplicationException ex)
        {
            var exceptionDictionary = new Dictionary<string, object>()
            {
                ["title"] = ex.Message
            };

            if (_environment.IsDevelopment())
            {
                exceptionDictionary["StackTrace"] = ex.StackTrace ?? "";
            }

            await GenerateResponse(context, ex.HttpStatusCode, exceptionDictionary);
        }
        catch (UnauthorizedAccessException ex)
        {
            var exceptionDictionary = new Dictionary<string, object>()
            {
                ["title"] = ex.Message
            };

            if (_environment.IsDevelopment())
            {
                exceptionDictionary["StackTrace"] = ex.StackTrace ?? "";
            }

            await GenerateResponse(context, HttpStatusCode.Unauthorized, exceptionDictionary);
        }
        catch (Exception ex)
        {
            var exceptionDictionary = new Dictionary<string, object>()
            {
                ["title"] = ex.Message
            };

            if (_environment.IsDevelopment())
            {
                exceptionDictionary["StackTrace"] = ex.StackTrace ?? "";
            }

            if (ex.Message.Contains("IDX12729"))
            {
                await GenerateResponse(context, HttpStatusCode.Unauthorized, exceptionDictionary);
            }
            else
                await GenerateResponse(context, HttpStatusCode.InternalServerError, exceptionDictionary);
        }
    }

    private static async Task GenerateResponse(HttpContext context, HttpStatusCode statusCode,
        Dictionary<string, object> messages)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(messages));
    }
}