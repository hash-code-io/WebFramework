using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HashCode.SharedKernel.Auth;

internal sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private DefaultAuthorizationPolicyProvider DefaultPolicyProvider { get; } = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => DefaultPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(AppPermission.Prefix, StringComparison.OrdinalIgnoreCase))
            return DefaultPolicyProvider.GetPolicyAsync(policyName);

        AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(AppPermission.From(policyName)))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => DefaultPolicyProvider.GetFallbackPolicyAsync();
}