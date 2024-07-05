using System.Diagnostics.CodeAnalysis;

namespace HashCode.SharedKernel.Domain;

public interface IEntity<out TId> : IDomainEventHolder where TId : struct, IEquatable<TId>
{
    TId Id { get; }
}

public abstract class Entity<TId>(TId id) : IEntity<TId>
    where TId : struct, IEquatable<TId>
{
    private readonly List<DomainEvent> _domainEvents = [];
    public TId Id { get; private init; } = id;

    [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "By Design")]
    IEnumerable<DomainEvent> IDomainEventHolder.RetrieveAndClearDomainEvents()
    {
        DomainEvent[] domainEventCopy = [.._domainEvents];
        _domainEvents.Clear();
        return domainEventCopy;
    }

    public bool HasDomainEvents => _domainEvents.Count > 0;

    [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "<Pending>")]
    protected void Raise(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}