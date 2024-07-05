using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace HashCode.SharedKernel;

[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
[SuppressMessage("Design", "CA1724:Type names should not match namespaces", Justification = "<Pending>")]
public static class AppConstants
{
    public static class CookieNames
    {
        public const string CultureCookie = ".AspNetCore.Culture";
        public const string ThemeCookie = "NgAisto.Theme";
        public const string BreakpointCookie = "NgAisto.Breakpoints";
    }

    public static class Auth
    {
        public const string UserServiceOidcScheme = "UserServiceOidc";
        public const string ApplicationScheme = "Cookies";
        public const string BearerScheme = "Bearer";
        public const string BearerOrOidcScheme = "BearerOrOidc";

        public static class Roles
        {
            public const string Admin = nameof(Admin);

            public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>([Admin]);
            public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
        }

        public static class ClaimTypes
        {
            public const string Permission = "x-permission";
        }
    }
}