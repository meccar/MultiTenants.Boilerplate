using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Helpers;

public static class IdentityErrorHelper
{
    public static Dictionary<string, string[]> ToErrorDictionary(IEnumerable<IdentityError> errors)
    {
        return errors
            .GroupBy(error => 
            {
                var parts = Regex.Split(error.Code, @"(?<!^)(?=[A-Z])");
                
                return parts.Length > 0 ? parts[0] : "General";
            })
            .ToDictionary(
                group => group.Key,
                group => group.Select(e => e.Code).ToArray()
            );
    }
}