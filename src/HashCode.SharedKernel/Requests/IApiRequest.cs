using HashCode.SharedKernel.Domain;
using System.Diagnostics.CodeAnalysis;

namespace HashCode.SharedKernel.Requests;

//NOTE: If you change anything about the namespace / name / generic params in this file you also have to update the EndpointGenerator!

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Not relevant")]
public interface IApiRequest<in TRequest> where TRequest : IApiRequest<TRequest>
{
    static abstract HttpConfiguration Configuration { get; }
    IApiHttpRequestMessageBuilder<TRequest> ApiHttpMessageBuilder => AutoConfigurationApiHttpRequestMessageBuilder<TRequest>.Instance;
    HttpRequestMessage Convert() => ApiHttpMessageBuilder.Build((TRequest)this);
}

public interface IApiCommandRequest<in TRequest, TResponse> : IAppCommand<TResponse>, IApiRequest<TRequest>
    where TRequest : IApiCommandRequest<TRequest, TResponse>;

public interface IApiCommandRequest<in TRequest> : IAppCommand, IApiRequest<TRequest> where TRequest : IApiCommandRequest<TRequest>;

public interface IApiQueryRequest<in TRequest, TResponse> : IAppQuery<TResponse>, IApiRequest<TRequest>
    where TRequest : IApiQueryRequest<TRequest, TResponse>;