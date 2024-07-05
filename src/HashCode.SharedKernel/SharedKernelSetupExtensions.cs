using HashCode.SharedKernel.Auth;
using HashCode.SharedKernel.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace HashCode.SharedKernel;

public static class SharedKernelSetupExtensions
{
    public static IServiceCollection SetUpSharedKernelServices(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventPublisher, MediatRDomainEventPublisher>();
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}