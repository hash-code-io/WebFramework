using System.Diagnostics.CodeAnalysis;

namespace HashCode.SharedKernel.Requests;

//NOTE: If you change anything about the namespace / name / generic params in this file you also have to update the EndpointGenerator!

public delegate Task<TResponse> ExternalApiCall<in TRequest, TResponse, in TClient>(TClient client, TRequest request, CancellationToken cancellationToken);

public delegate Task ExternalApiCall<in TRequest, in TClient>(TClient client, TRequest request, CancellationToken cancellationToken);

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Not relevant")]
public interface IExternalApiRequest<in TRequest, TResponse, in TClient> : IApiRequest<TRequest>
    where TRequest : IExternalApiRequest<TRequest, TResponse, TClient>
    where TClient: notnull
{
    static abstract ExternalApiCall<TRequest, TResponse, TClient> ExternalApiCall { get; }
}

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Not relevant")]
public interface IExternalApiRequest<in TRequest, in TClient> : IApiRequest<TRequest>
    where TRequest : IExternalApiRequest<TRequest, TClient>
    where TClient : notnull
{
    static abstract ExternalApiCall<TRequest, TClient> ExternalApiCall { get; }
}