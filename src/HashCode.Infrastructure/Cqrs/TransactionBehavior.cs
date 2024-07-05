using HashCode.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HashCode.Infrastructure.Cqrs;

internal sealed class TransactionBehavior<TRequest, TResponse>(IServiceProvider sp, IOptions<DbContextOptions> options) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAppBaseCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var dbContext = (DbContext)sp.GetRequiredService(options.Value.DbContextType);
        if (dbContext.Database.CurrentTransaction is not null)
            return await next().ConfigureAwait(false);

        IExecutionStrategy execStrategy = dbContext.Database.CreateExecutionStrategy();
        return await execStrategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction tran = await dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                TResponse response = await next().ConfigureAwait(false);
                await tran.CommitAsync(cancellationToken).ConfigureAwait(false);
                return response;
            }
            catch
            {
                await tran.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }).ConfigureAwait(false);
    }
}