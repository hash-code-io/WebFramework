using FastEndpoints;
using HashCode.SharedKernel.Extension;
using HashCode.SharedKernel.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;

namespace HashCode.SharedKernel.Requests;

public class AutoConfigurationApiHttpRequestMessageBuilder<TApiRequest> : IApiHttpRequestMessageBuilder<TApiRequest>
    where TApiRequest : IApiRequest<TApiRequest>
{
    private static AutoConfigurationApiHttpRequestMessageBuilder<TApiRequest>? _instance;
    private readonly Dictionary<string, QueryParamInfo> _queryParamRetrievers = [];
    private readonly List<Func<TApiRequest, object>> _routeParamRetrievers = [];
    private CompositeFormat _routeFormatString;

    private AutoConfigurationApiHttpRequestMessageBuilder() => ParseRouteAndQueryParams();

    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Not relevant")]
    public static AutoConfigurationApiHttpRequestMessageBuilder<TApiRequest> Instance => _instance ??= new AutoConfigurationApiHttpRequestMessageBuilder<TApiRequest>();

    private static HttpConfiguration HttpConfiguration => TApiRequest.Configuration;

    public HttpRequestMessage Build(TApiRequest apiRequest) => new(HttpConfiguration.HttpMethod, BuildRoute(apiRequest)) { Content = BuildBody(apiRequest) };

    [MemberNotNull(nameof(_routeFormatString))]
    private void ParseRouteAndQueryParams()
    {
        (string replacementRoute, IReadOnlyList<string> orderedParamNames) = RouteTemplateParser.Parse(HttpConfiguration.RouteTemplate);
        _routeFormatString = CompositeFormat.Parse(replacementRoute);

        foreach (PropertyInfo propertyInfo in typeof(TApiRequest).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            bool addedToRouteParams = false;
            if (orderedParamNames.Contains(propertyInfo.Name.ToLowerInvariant()))
            {
                _routeParamRetrievers.Add(request => propertyInfo.GetValue(request)
                                                     ?? throw new InvalidOperationException($"Route Param Property with name [{propertyInfo.Name}] was null")
                );
                addedToRouteParams = true;
            }

            if (propertyInfo.GetCustomAttribute<QueryParamAttribute>() is not null)
            {
                if (addedToRouteParams)
                    throw new InvalidOperationException($"Parameter with name {propertyInfo.Name} was found to be declared both as a route parameter and a query parameter");

                _queryParamRetrievers.Add(propertyInfo.Name, new QueryParamInfo(propertyInfo.PropertyType.IsTriviallySerializable(), request => propertyInfo.GetValue(request))
                );
            }
        }

        if (_routeParamRetrievers.Count != orderedParamNames.Count)
            throw new InvalidOperationException($"Could not find all properties on Request. Requested properties were: [{string.Join(',', orderedParamNames)}]");
    }

    private string BuildRoute(TApiRequest apiRequest)
    {
        string formattedRoute = string.Format(null, _routeFormatString!, [.. _routeParamRetrievers.Select(retriever => retriever(apiRequest))]);
        if (_queryParamRetrievers.Count == 0)
            return formattedRoute;

        List<string> queryParams = [];

        foreach ((string key, QueryParamInfo info) in _queryParamRetrievers)
        {
            string? value = RetrieveQueryParamValue(info, apiRequest);
            if (value is null) continue;
            queryParams.Add($"{key}={value}");
        }

        return queryParams.Count == 0 ? formattedRoute : $"{formattedRoute}?{string.Join("&", queryParams)}";

        static string? RetrieveQueryParamValue(QueryParamInfo queryParamInfo, TApiRequest request)
        {
            object? value = queryParamInfo.Retriever(request);
            return value is null
                ? null
                : queryParamInfo.IsSimple
                    ? value.ToString()
                    : AppSerializer.Serialize(value);
        }
    }

    private static JsonContent? BuildBody(TApiRequest apiRequest) => HttpConfiguration.HasBody ? JsonContent.Create(apiRequest, null, AppSerializer.Options) : null;

    private record QueryParamInfo(bool IsSimple, Func<TApiRequest, object?> Retriever);
}