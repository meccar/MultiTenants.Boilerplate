namespace BuildingBlocks.Endpoints;

public sealed class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public sealed class UpdateManageInfoRequest
{
    public string? Email { get; set; }
    public string? UserName { get; set; }
}

public sealed class DeletePersonalDataRequest
{
    public string Password { get; set; } = string.Empty;
}
