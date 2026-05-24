using BuildingBlocks.Shared.Constants;

namespace BuildingBlocks.Shared.Exceptions;

public class BadRequestException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public BadRequestException()
        : base(ResponseMessageConstants.BadRequest)
    {
        Errors = [];
    }

    public BadRequestException(string message)
        : base(message)
    {
        Errors = [];
    }

    public BadRequestException(string key, string errorCode)
        : base(ResponseMessageConstants.BadRequest)
    {
        Errors = new Dictionary<string, string[]>
        {
            { key, [errorCode] }
        };
    }

    public BadRequestException(Dictionary<string, string[]> errors)
        : base(ResponseMessageConstants.BadRequest)
    {
        Errors = errors;
    }
}