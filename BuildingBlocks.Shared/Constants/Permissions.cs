namespace BuildingBlocks.Shared.Constants
{
    public static class Permissions
    {
        public static class Users
        {
            public const string View = "users:view";
            public const string Create = "users:create";
            public const string Edit = "users:edit";
            public const string ChangePassword = "users:change-password";
            public const string Ban = "users:ban";
        }

        public static class Orders
        {
            public const string View = "orders:view";
            public const string Create = "orders:create";
            public const string Edit = "orders:edit";
            public const string Delete = "orders:delete";
        }

        public static class Reports
        {
            public const string View = "reports:view";
            public const string Export = "reports:export";
        }
    }
}
