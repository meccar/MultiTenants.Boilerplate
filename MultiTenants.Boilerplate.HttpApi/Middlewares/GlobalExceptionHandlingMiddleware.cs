using MultiTenants.Boilerplate.Shared.Constants;
using MultiTenants.Boilerplate.Shared.Responses;
using System;

namespace MultiTenants.Boilerplate.Middlewares;
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlingMiddleware> logger
    ){
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context, Exception exception
    ) {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>();
        response.TraceId = context.TraceIdentifier;

        switch(exception)
        {
            case ArgumentNullException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = ApiResponse<object>.BadRequest(
                    message: exception.Message
                );
                break;
            case ArgumentException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = ApiResponse<object>.BadRequest(
                    message: exception.Message
                );
                break;
            case InvalidOperationException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = ApiResponse<object>.FailureResponse(
                    statusCode: System.Net.HttpStatusCode.Conflict,
                    message: exception.Message
                );
                break;
            case KeyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = ApiResponse<object>.NotFound(
                    message: ResponseMessageConstants.NotFound
                );
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = ApiResponse<object>.Unauthorized(
                    message: exception.Message
                );
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = ApiResponse<object>.InternalServerError(
                    message: ResponseMessageConstants.InternalServerError
                );
                break;
        }

        response.StatusCode = (System.Net.HttpStatusCode)context.Response.StatusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}

public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
