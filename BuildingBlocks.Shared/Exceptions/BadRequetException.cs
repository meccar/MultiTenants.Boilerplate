using BuildingBlocks.Shared.Constants;

namespace BuildingBlocks.Shared.Exceptions;

public class BadRequetException : Exception
{
    public Dictionary<string, string[]> Errors { get; }
    public BadRequetException(
        Dictionary<string, string[]> errors) 
        : base(ResponseMessageConstants.BadRequest)
    {
        Errors = errors;
    }
    public BadRequetException(string key, string errorCode) 
        : base(ResponseMessageConstants.BadRequest)
    {
        Errors = new Dictionary<string, string[]>
        {
            { key, new[] { errorCode } }
        };
    }
}
