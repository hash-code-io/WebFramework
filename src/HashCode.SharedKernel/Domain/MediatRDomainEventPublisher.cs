using MediatR;

namespace HashCode.SharedKernel.Domain;

internal sealed class MediatRDomainEventPublisher(IPublisher publisher) : IDomainEventPublisher
{
    public Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default) => publisher.Publish(domainEvent, cancellationToken);
}