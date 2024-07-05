using HashCode.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace HashCode.Infrastructure.EntityFramework;

internal static class EfCoreExtensions
{
    public static IEnumerable<DomainEvent> BuildModificationEvents(this ChangeTracker changeTracker) =>
        changeTracker.Entries<IAggregateRoot>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => e.State switch
                {
                    EntityState.Deleted => AggregateRootModifiedEvent.Create(EntityModificationState.Deleted, e.Entity),
                    EntityState.Modified => AggregateRootModifiedEvent.Create(EntityModificationState.Updated, e.Entity),
                    EntityState.Added => AggregateRootModifiedEvent.Create(EntityModificationState.New, e.Entity),
                    _ => throw new InvalidOperationException($"Cannot build modification event for entity with state {Enum.GetName(e.State)}")
                }
            );

    public static IEnumerable<DomainEvent> FindDomainEvents(this ChangeTracker changeTracker) =>
        changeTracker.Entries<IDomainEventHolder>()
            .Select(e => e.Entity)
            .Where(e => e.HasDomainEvents)
            .SelectMany(x => x.RetrieveAndClearDomainEvents());

    public static async Task<T> RunInTransaction<T>(this DbContext ctx, Func<Task<T>> action, CancellationToken cancellationToken)
    {
        if (ctx.Database.CurrentTransaction is not null) return await action();

        IExecutionStrategy execStrategy = ctx.Database.CreateExecutionStrategy();
        return await execStrategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction tran = await ctx.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                T result = await action();
                await tran.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await tran.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public static Task RunInTransaction(this DbContext ctx, Func<Task> action, CancellationToken cancellationToken)
        => ctx.RunInTransaction(async () =>
        {
            await action();
            return Unit.Value;
        }, cancellationToken);
}