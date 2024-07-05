using Ardalis.Specification;

namespace HashCode.SharedKernel.Domain;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot;