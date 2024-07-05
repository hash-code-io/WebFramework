using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace HashCode.Infrastructure.Cqrs;

internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<Mediator> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!logger.IsEnabled(LogLevel.Debug))
            return await next();

        logger.LogDebug("Handling {RequestName}", typeof(TRequest).Name);

        foreach (PropertyInfo prop in request.GetType().GetProperties())
        {
            object? propValue = prop.GetValue(request, null);
            logger.LogDebug("Property {Property} : {Value}", prop.Name, propValue);
        }

        var sw = Stopwatch.StartNew();
        TResponse response = await next();
        sw.Stop();

        logger.LogDebug("Handled {RequestName} with {Response} in {Ms} ms", typeof(TRequest).Name, response, sw.ElapsedMilliseconds);
        
        return response;
    }
}