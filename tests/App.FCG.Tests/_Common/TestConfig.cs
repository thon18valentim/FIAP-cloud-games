using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace FCG.Tests._Common;

public static class TestConfig
{
    public static readonly PasswordOptions PasswordPolicy = new()
    {
        RequiredLength = 8,
        RequireNonAlphanumeric = true,
        RequireDigit = true,
        RequireUppercase = true,
        RequireLowercase = false,
        RequiredUniqueChars = 1
    };

    // Opcional, caso queira validação por regex
    public static readonly Regex PasswordRegex =
        new(@"^(?=(?:.*\d){3,})(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$",
            RegexOptions.Compiled);
}
