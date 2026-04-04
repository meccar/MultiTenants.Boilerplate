using NUlid;

namespace BuildingBlocks.Shared.Utilities;

public static class UlidGenerator
{
    public static string NewUlid() => Ulid.NewUlid().ToString();
}

