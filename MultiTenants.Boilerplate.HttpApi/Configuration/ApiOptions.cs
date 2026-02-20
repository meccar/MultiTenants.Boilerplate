namespace MultiTenants.Boilerplate.Configurations;

/// <summary>
/// API version and base path options. Bound from configuration "Api" section.
/// </summary>
public class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>
    /// API version (e.g. "v1", "v2"). Used in the base path and Swagger doc.
    /// </summary>
    public string Version { get; set; } = "v1";

    /// <summary>
    /// Base path for all API routes, e.g. "/api/v1". Derived from Version when not set.
    /// </summary>
    public string BasePath => string.IsNullOrEmpty(Version) ? "/api" : $"/api/{Version.Trim('/')}";
}
