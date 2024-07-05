using MediatR;

namespace HashCode.SharedKernel.Domain;

public abstract record DomainEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}