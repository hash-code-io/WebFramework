using Microsoft.AspNetCore.Authorization;

namespace HashCode.SharedKernel.Auth;

internal sealed record PermissionRequirement(AppPermission Permission) : IAuthorizationRequirement;