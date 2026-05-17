using System.Net;
using BuildingBlocks.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Host.Filters;

public class ApiResponseFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value != null && 
                (objectResult.Value.GetType().IsGenericType && 
                 objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>)) ||
                 objectResult.Value is ApiResponse)
                return;

            var statusCode = (HttpStatusCode)(objectResult.StatusCode ?? 200);
            
            bool isSuccess = objectResult.StatusCode >= 200 && objectResult.StatusCode < 300;

            object wrappedResponse;

            if (isSuccess)
            {
                var responseType = typeof(ApiResponse<>).MakeGenericType(objectResult.Value?.GetType() ?? typeof(object));
                wrappedResponse = Activator.CreateInstance(responseType, objectResult.Value, null, statusCode)!;
            }
            else
            {
                Dictionary<string, string[]>? errors = null;

                if (objectResult.Value is ValidationProblemDetails problemDetails)
                    errors = problemDetails.Errors.ToDictionary(k => k.Key, v => v.Value);
                else if (objectResult.Value is SerializableError serializableError)
                    errors = serializableError.ToDictionary(k => k.Key, v => (string[])v.Value);

                wrappedResponse = ApiResponse<object>.FailureResponse(statusCode, "An error occurred", errors);
            }

            objectResult.Value = wrappedResponse;
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}