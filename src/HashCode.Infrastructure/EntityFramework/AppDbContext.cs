using HashCode.Infrastructure.EntityFramework.ValueConverters;
using HashCode.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace HashCode.Infrastructure.EntityFramework;

public abstract class AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher? dispatcher) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.ConfigureSmartEnum(); // This MUST come first for some reason. Might be a good idea to implmenet myself
        configurationBuilder.Properties<decimal>().HavePrecision(16, 2);
        configurationBuilder.ApplyValueConvertersFromAssemblies(typeof(AppDbContext), GetType());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await this.RunInTransaction(async () =>
        {
            List<DomainEvent> modificationEvents = [.. ChangeTracker.BuildModificationEvents()];
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Dispatch events AFTER successful save
            await DispatchEvents(modificationEvents, cancellationToken);

            return result;
        }, cancellationToken);

    public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

    private async Task DispatchEvents(IEnumerable<DomainEvent> modificationEvents, CancellationToken cancellationToken)
    {
        // ignore events if no dispatcher provided
        if (dispatcher is null) return;

        IEnumerable<DomainEvent> events = ChangeTracker.FindDomainEvents();
        await dispatcher.Dispatch(events, cancellationToken);
        await dispatcher.Dispatch(modificationEvents, cancellationToken);
    }
}