namespace HashCode.SharedKernel.Domain;

public interface IDomainEventHolder
{
    IEnumerable<DomainEvent> RetrieveAndClearDomainEvents();
    bool HasDomainEvents { get; }
}