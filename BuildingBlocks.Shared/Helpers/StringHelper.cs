namespace BuildingBlocks.Shared.Helpers;
public class StringHelper
{
    public static string MaskInput(string input)
        => input.Length > 3
            ? input[..3] + "***" 
            : "***";
}
