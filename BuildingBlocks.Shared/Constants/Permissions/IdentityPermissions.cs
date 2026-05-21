namespace BuildingBlocks.Shared.Constants.Permissions;

public class IdentityPermissions
{
    public const string UsersRead   = "Identity:users:read";
    public const string UsersWrite  = "Identity:users:write";
    public const string UsersDelete = "Identity:users:delete";

    public const string RolesRead   = "Identity:roles:read";
    public const string RolesWrite  = "Identity:roles:write";
    public const string RolesDelete = "Identity:roles:delete";
    
    public const string AccountsCreateUserAccount = "Identity:Accounts:CreateUserAccount";
    public const string AccountsLogout = "Identity:Accounts:Logout";
    public const string AccountsChangeLoggedOutUserPassword = "Identity:Accounts:ChangeLoggedOutUserPassword";
    public const string AccountsChangePasswordByOwner = "Identity:Accounts:ChangePasswordByOwner";
    public const string AccountsChangeLoggedInUserPassword = "Identity:Accounts:ChangeLoggedInUserPassword";
    
}