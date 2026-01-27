using NUlid;

namespace MultiTenants.Boilerplate.Shared.Utilities;

public static class UlidGenerator
{
    public static string NewUlid() => Ulid.NewUlid().ToString();
}

