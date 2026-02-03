using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MultiTenants.Boilerplate.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger
    ) {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    ) {
        if (!_validators.Any()) {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count > 0)
        {
            _logger.LogWarning(
                "Validation errors - {RequestType} - Errors: {@ValidationErrors} - Request: {@Request}",
                typeof(TRequest).Name,
                failures,
                request
            );

            var errorDictionary = failures
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            if (typeof(TResponse).IsGenericType 
                && typeof(TResponse).GetGenericTypeDefinition().Name.Contains("Result"))
            {
                var resultType = typeof(TResponse);
                var failureMethod = resultType.GetMethod("Failure", 
                    System.Reflection.BindingFlags.Static 
                    | System.Reflection.BindingFlags.Public, null, [typeof(string)], null);

                if (failureMethod != null)
                {
                    var errorMessage = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
                    return (TResponse)failureMethod.Invoke(null, [errorMessage])!;
                }
            }
            throw new ValidationException(failures);
        }
        return await next(cancellationToken);
    }
}
