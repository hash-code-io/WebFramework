using Microsoft.AspNetCore.Authorization;

namespace HashCode.SharedKernel.Auth;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasPermissionClaim(requirement.Permission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}