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
        HttpContext ctx, Exception ex
    ) {
        ctx.Response.ContentType = "application/json";

        var response = new ApiResponse<object>();
        response.TraceId = ctx.TraceIdentifier;

        switch(ex)
        {
            case ArgumentNullException:
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = ApiResponse<object>.BadRequest(
                    message: ex.Message
                );
                break;
            case ArgumentException:
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = ApiResponse<object>.BadRequest(
                    message: ex.Message
                );
                break;
            case InvalidOperationException:
                ctx.Response.StatusCode = StatusCodes.Status409Conflict;
                response = ApiResponse<object>.FailureResponse(
                    statusCode: System.Net.HttpStatusCode.Conflict,
                    message: ex.Message
                );
                break;
            case KeyNotFoundException:
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                response = ApiResponse<object>.NotFound(
                    message: ResponseMessageConstants.NotFound
                );
                break;
            case UnauthorizedAccessException:
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = ApiResponse<object>.Unauthorized(
                    message: ex.Message
                );
                break;
            default:
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = ApiResponse<object>.InternalServerError(
                    message: ResponseMessageConstants.InternalServerError
                );
                break;
        }

        response.StatusCode = (System.Net.HttpStatusCode)ctx.Response.StatusCode;
        return ctx.Response.WriteAsJsonAsync(response);
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
