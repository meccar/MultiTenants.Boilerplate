using Identity.Domain.Records;

namespace Identity.Domain.Interfaces;

public interface IPasswordResetEmailJob
{
    Task EnqueueAsync(string email, string callbackUrl, CancellationToken cancellationToken);
}