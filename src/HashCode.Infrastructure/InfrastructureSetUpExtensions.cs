using HashCode.Infrastructure.Cqrs;
using HashCode.Infrastructure.EntityFramework;
using HashCode.SharedKernel.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace HashCode.Infrastructure;

public static class InfrastructureSetUpExtensions
{
    public static IServiceCollection SetUpInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
        services.SetUpMediatR();
        services.AddScoped<IReadModelRepository, ReadModelRepository>();
        services.SetUpAuth();

        return services;
    }

    private static void SetUpMediatR(this IServiceCollection services) =>
        services.AddMediatR(o =>
        {
            o.RegisterServicesFromAssemblies(
                typeof(GerpDatasheet).Assembly, //Core
                typeof(AppDbContext).Assembly, // Infrastructure
                typeof(UseCaseAssemblyMarker).Assembly // UseCases
            );

            o.AddOpenBehavior(typeof(LoggingBehavior<,>));
            o.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });
}