namespace HashCode.SharedKernel.Domain;

public interface IDomainEventDispatcher
{
    Task Dispatch(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
    Task Dispatch(DomainEvent @event, CancellationToken cancellationToken = default);
}