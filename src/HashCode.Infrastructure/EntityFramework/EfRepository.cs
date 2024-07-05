using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HashCode.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HashCode.Infrastructure.EntityFramework;

internal sealed class EfRepository<T>(IServiceProvider sp, IOptions<DbContextOptions> options, IMapper mapper) : RepositoryBase<T>((AppDbContext)sp.GetRequiredService(options.Value.DbContextType)),
    IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{
    public async Task<PagedDataResult<TResult>> MapToPagedDataResult<TResult>(ISpecification<T> specification, IPageableQuery pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        int total = await CountAsync(specification, cancellationToken);

        List<T> entities = await ApplySpecification(specification)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToListAsync(cancellationToken);

        List<TResult> data = mapper.Map<List<TResult>>(entities);
        return new PagedDataResult<TResult>(total, data);
    }

    public async Task<PagedDataResult<TResult>> ProjectToPagedDataResult<TResult>(ISpecification<T> specification, IPageableQuery pagination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        int total = await CountAsync(specification, cancellationToken);

        List<TResult> data = await ApplySpecification(specification)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ProjectTo<TResult>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedDataResult<TResult>(total, data);
    }

    public async Task<TResult?> MapToSingleResult<TResult>(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        T? data = await SingleOrDefaultAsync(specification, cancellationToken); // TODO: evaluate if FirstOrDefault is better here
        return mapper.Map<TResult>(data);
    }

    public async Task<TResult?> ProjectToSingleResult<TResult>(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default) =>
        await ApplySpecification(specification)
            .ProjectTo<TResult>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(cancellationToken); // TODO: evaluate if FirstOrDefault is better here
}