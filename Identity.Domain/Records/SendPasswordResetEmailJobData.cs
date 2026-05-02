namespace Identity.Domain.Records;

public record SendPasswordResetEmailJobData(
    string Email,
    string CallbackUrl
);