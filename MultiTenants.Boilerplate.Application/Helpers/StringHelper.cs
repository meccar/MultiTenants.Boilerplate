namespace MultiTenants.Boilerplate.Application.Helpers;
public class StringHelper
{
    public static string MaskInput(string input)
        => input.Length > 3
            ? input[..3] + "***" 
            : "***";
}
