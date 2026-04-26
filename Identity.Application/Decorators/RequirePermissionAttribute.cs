namespace Identity.Application.Decorators
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute 
        : Attribute
    {
        public string Permission { get; }

        public RequirePermissionAttribute(
            string permission
        ){
            Permission = permission;
        }
    }
}
