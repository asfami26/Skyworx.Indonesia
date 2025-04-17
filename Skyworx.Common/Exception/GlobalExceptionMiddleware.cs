using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Skyworx.Common.Constants;

namespace Skyworx.Common.Exception;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, ResponseConstant.Unauthorize);
            await HandleExceptionAsync(context, 401, ex.Message, ex.InnerException?.Message);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, ResponseConstant.InvalidInput);
            await HandleExceptionAsync(context, 400, ex.Message, ex.InnerException?.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, ResponseConstant.MesNotFound);
            await HandleExceptionAsync(context, 404, ex.Message, ex.InnerException?.Message);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception");
            await HandleExceptionAsync(context, 500, "Internal Server Error", ex.InnerException?.Message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, int statusCode, string message, string? inner = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            message,
            innerException = inner
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}