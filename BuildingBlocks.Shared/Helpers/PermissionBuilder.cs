namespace BuildingBlocks.Shared.Helpers;

public static class PermissionBuilder
{
    public static string Build(string resource, string action)
        => $"{resource}:{action}";

    public static string Build(string resource, string action, string scope)
        => $"{resource}:{action}:{scope}";
}