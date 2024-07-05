namespace HashCode.SharedKernel.Requests;

public interface IApiHttpRequestMessageBuilder<in TApiRequest> where TApiRequest : IApiRequest<TApiRequest>
{
    HttpRequestMessage Build(TApiRequest apiRequest);
}