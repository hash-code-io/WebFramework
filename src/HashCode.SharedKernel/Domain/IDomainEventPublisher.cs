namespace HashCode.SharedKernel.Domain;

public interface IDomainEventPublisher
{
    Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}