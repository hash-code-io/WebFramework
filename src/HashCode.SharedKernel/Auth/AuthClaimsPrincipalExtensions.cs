using System.Security.Claims;

namespace HashCode.SharedKernel.Auth;

public static class AuthClaimsPrincipalExtensions
{
    public static bool HasPermissionClaim(this ClaimsPrincipal principal, AppPermission permission)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return principal.HasClaim(AppConstants.Auth.ClaimTypes.Permission, permission);
    }

    public static bool HaAnyPermissionClaim(this ClaimsPrincipal principal, params AppPermission[] permissions)
        => permissions.Any(permission => HasPermissionClaim(principal, permission));
}