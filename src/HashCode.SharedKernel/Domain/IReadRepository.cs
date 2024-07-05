using Ardalis.Specification;
using HashCode.SharedKernel.Exceptions;

namespace HashCode.SharedKernel.Domain;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
    Task<PagedDataResult<TResult>> MapToPagedDataResult<TResult>(ISpecification<T> specification, IPageableQuery pagination, CancellationToken cancellationToken = default);
    Task<PagedDataResult<TResult>> ProjectToPagedDataResult<TResult>(ISpecification<T> specification, IPageableQuery pagination, CancellationToken cancellationToken = default);

    Task<TResult?> MapToSingleResult<TResult>(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default);
    Task<TResult?> ProjectToSingleResult<TResult>(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default);
}

public static class RepositoryResultExtensions
{
    public static async Task<TResult> Required<TResult>(this Task<TResult?> resultTask)
        => await resultTask ?? throw new AppException($"Required entity of type {typeof(TResult).Name} was not found");
}