namespace HashCode.SharedKernel.Domain;

public record AggregateRootModifiedEvent : DomainEvent
{
    private static readonly Type GenericEventType = typeof(AggregateRootModifiedEvent<>);
    public static AggregateRootModifiedEvent Create(EntityModificationState state, IAggregateRoot aggregateRoot)
    {
        ArgumentNullException.ThrowIfNull(aggregateRoot);
        Type aggregateType = aggregateRoot.GetType();
        Type actualEventType = GenericEventType.MakeGenericType(aggregateType);
        return (AggregateRootModifiedEvent)Activator.CreateInstance(actualEventType, [state, aggregateRoot])!;
    }
}

// if you ever change the constructor also change AggregateRootModifiedEvent's reflection-based Create function
public sealed record AggregateRootModifiedEvent<T>(EntityModificationState State, T AggregateRoot) : AggregateRootModifiedEvent
    where T : IAggregateRoot;