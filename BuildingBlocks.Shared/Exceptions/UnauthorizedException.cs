using BuildingBlocks.Shared.Constants;

namespace BuildingBlocks.Shared.Exceptions;

public class UnauthorizedException : Exception
{
    public Dictionary<string, string[]> Errors { get; }
    
    public UnauthorizedException()
        : base(ResponseMessageConstants.Unauthorized)
    {
        Errors = new Dictionary<string, string[]>();
    }
    public UnauthorizedException(
        Dictionary<string, string[]> errors)
        : base(ResponseMessageConstants.Unauthorized)
    {
        Errors = errors;
    }
    public UnauthorizedException(string key, string errorCode) 
        : base(ResponseMessageConstants.Unauthorized)
    {
        Errors = new Dictionary<string, string[]>
        {
            { key, new[] { errorCode } }
        };
    }
}