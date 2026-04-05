namespace Identity.Domain.Models
{
    public class UserResult
    {
        public bool Succeeded { get; init; }
        public IEnumerable<string> Errors { get; init; } = [];

        public static UserResult Success() => new() { Succeeded = true };
        public static UserResult Failure(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors };
    }
}
