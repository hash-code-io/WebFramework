using MediatR;

namespace HashCode.SharedKernel.Domain;

public interface IDenormalizer
{
    Task RestoreAllReadModels(CancellationToken cancellationToken = default);
}

public interface IDenormalizer<TAggregateRoot> : IDenormalizer, INotificationHandler<AggregateRootModifiedEvent<TAggregateRoot>>
    where TAggregateRoot : IAggregateRoot;