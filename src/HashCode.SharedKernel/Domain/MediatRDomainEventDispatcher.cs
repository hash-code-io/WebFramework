namespace HashCode.SharedKernel.Domain;

internal sealed class MediatRDomainEventDispatcher(IDomainEventPublisher publisher) : IDomainEventDispatcher
{
    public async Task Dispatch(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (DomainEvent domainEvent in events)
            await publisher.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
    }

    public async Task Dispatch(DomainEvent @event, CancellationToken cancellationToken = default)
        => await publisher.Publish(@event, cancellationToken).ConfigureAwait(false);
}