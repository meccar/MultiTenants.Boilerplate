using MultiTenants.Boilerplate.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MultiTenants.Boilerplate.Shared.Responses;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
    public ApiResponse() {}
    public ApiResponse(
        T? data,
        string? message = null,
        HttpStatusCode statusCode = HttpStatusCode.OK
    )
    {
        Success = true;
        Data = data;
        Message = message ?? ResponseMessageConstants.Success;
        StatusCode = statusCode;
    }
    public ApiResponse(
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? message = null,
        Dictionary<string, string[]>? errors = null
    )
    {
        Success = false;
        StatusCode = statusCode;
        Message = message ?? ResponseMessageConstants.BadRequest;
        Errors = errors;
    }
    public static ApiResponse<T> SuccessResponse(
        T? data,
        string? message = null,
        HttpStatusCode statusCode = HttpStatusCode.OK
    ) => new(data, message, statusCode);
    public static ApiResponse<T> FailureResponse(
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? message = null,
        Dictionary<string, string[]>? errors = null
    ) => new(statusCode, message, errors);
    public static ApiResponse<T> BadRequest(
        string? message = null,
        Dictionary<string, string[]>? errors = null
    ) => FailureResponse(HttpStatusCode.BadRequest, message, errors);
    public static ApiResponse<T> NotFound(
        string? message = ResponseMessageConstants.NotFound
    ) => FailureResponse(HttpStatusCode.NotFound, message);
    public static ApiResponse<T> Unauthorized(
        string? message = ResponseMessageConstants.Unauthorized
    ) => FailureResponse(HttpStatusCode.Unauthorized, message);
    public static ApiResponse<T> Forbidden(
        string? message = ResponseMessageConstants.Forbidden
    ) => FailureResponse(HttpStatusCode.Forbidden, message);
    public static ApiResponse<T> InternalServerError(
        string? message = ResponseMessageConstants.InternalServerError
    ) => FailureResponse(HttpStatusCode.InternalServerError, message);
    public static ApiResponse<T> ValidationError(
        Dictionary<string, string[]>? errors = null
    ) => FailureResponse(HttpStatusCode.BadRequest, ResponseMessageConstants.ValidationError, errors);
}

public class ApiResponse
{
    public bool Success { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
    public ApiResponse() { }
    public ApiResponse(
        string? message = null,
        HttpStatusCode statusCode = HttpStatusCode.OK
    )
    {
        Success = true;
        Message = message ?? ResponseMessageConstants.Success;
        StatusCode = statusCode;
    }
    public ApiResponse(
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? message = null,
        Dictionary<string, string[]>? errors = null
    )
    {
        Success = false;
        StatusCode = statusCode;
        Message = message ?? ResponseMessageConstants.BadRequest;
        Errors = errors;
    }
    public static ApiResponse SuccessResponse(
        string? message = null,
        HttpStatusCode statusCode = HttpStatusCode.OK
    ) => new(message, statusCode);
    public static ApiResponse FailureResponse(
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? message = null,
        Dictionary<string, string[]>? errors = null
    ) => new(statusCode, message, errors);
    public static ApiResponse BadRequest(
        string? message = null,
        Dictionary<string, string[]>? errors = null
    ) => FailureResponse(HttpStatusCode.BadRequest, message, errors);
    public static ApiResponse NotFound(
        string? message = ResponseMessageConstants.NotFound
    ) => FailureResponse(HttpStatusCode.NotFound, message);
    public static ApiResponse Unauthorized(
        string? message = ResponseMessageConstants.Unauthorized
    ) => FailureResponse(HttpStatusCode.Unauthorized, message);
    public static ApiResponse Forbidden(
        string? message = ResponseMessageConstants.Forbidden
    ) => FailureResponse(HttpStatusCode.Forbidden, message);
    public static ApiResponse InternalServerError(
        string? message = ResponseMessageConstants.InternalServerError
    ) => FailureResponse(HttpStatusCode.InternalServerError, message);
    public static ApiResponse ValidationError(
        Dictionary<string, string[]>? errors = null
    ) => FailureResponse(HttpStatusCode.BadRequest, ResponseMessageConstants.ValidationError, errors);
}
