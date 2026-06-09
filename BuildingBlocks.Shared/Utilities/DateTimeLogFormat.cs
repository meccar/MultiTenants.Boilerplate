namespace BuildingBlocks.Shared.Utilities;

public static class DateTimeLogFormat
{
    internal static string ISOLogFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

    public static string ISOFormat(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    public static string ISOFormat(this DateTimeOffset dateTime)
    {
        return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}