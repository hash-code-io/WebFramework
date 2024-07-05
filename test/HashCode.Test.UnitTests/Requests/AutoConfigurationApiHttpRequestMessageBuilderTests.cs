using FastEndpoints;
using HashCode.SharedKernel.Requests;
using System.Net.Http.Json;

namespace HashCode.Test.UnitTests.Requests;

public class AutoConfigurationApiHttpRequestMessageBuilderTests
{
    private static async Task RunTest<TRequestType>() where TRequestType : ITestRequest<TRequestType>
    {
        // Arrange
        TRequestType request = Activator.CreateInstance<TRequestType>();
        AutoConfigurationApiHttpRequestMessageBuilder<TRequestType> sut = AutoConfigurationApiHttpRequestMessageBuilder<TRequestType>.Instance;

        // Act
        using HttpRequestMessage httpRequestMessage = sut.Build(request);

        // Assert
        Assert.Equal(request.ExpectedRoute, httpRequestMessage.RequestUri!.ToString());
        if (TRequestType.Configuration.HasBody)
        {
            Assert.NotNull(httpRequestMessage.Content);
            Assert.Equal(typeof(JsonContent), httpRequestMessage.Content!.GetType());
            TRequestType? deserializedRequest = await httpRequestMessage.Content.ReadFromJsonAsync<TRequestType>();
            Assert.NotNull(deserializedRequest);
            Assert.Equal(request, deserializedRequest);
        }
        else
        {
            Assert.Null(httpRequestMessage.Content);
        }
    }

    [Fact]
    public Task Should_Build_Correct_Route_Get_Simple() => RunTest<SimpleGetRequest>();

    [Fact]
    public Task Should_Build_Correct_Route_Get_RouteParams() => RunTest<RouteParamGetRequest>();

    [Fact]
    public Task Should_Build_Correct_Route_Get_QueryParams() => RunTest<QueryParamGetRequest>();

    [Fact]
    public Task Should_Build_Correct_Route_Get_RouteAndQueryParams() => RunTest<RouteAndQueryParamGetRequest>();

    [Fact]
    public Task Should_Build_Correct_Route_Post_RouteAndQueryParams() => RunTest<RouteAndQueryParamPostRequest>();

    #region Models

    private interface ITestRequest<in TApiRequest> : IApiRequest<TApiRequest> where TApiRequest : IApiRequest<TApiRequest>
    {
        string ExpectedRoute { get; }
    }

    private sealed record SimpleGetRequest : ITestRequest<SimpleGetRequest>
    {
        public string ExpectedRoute => "gerp-datasheets";
        public static HttpConfiguration Configuration { get; } = HttpConfiguration.GetAnonymous("gerp-datasheets");
    }

    private sealed record RouteParamGetRequest : ITestRequest<RouteParamGetRequest>
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public int OtherId { get; init; } = 2;
        public string ExpectedRoute => $"gerp-datasheets/{Id}/derp/{OtherId}";
        public static HttpConfiguration Configuration { get; } = HttpConfiguration.GetAnonymous("gerp-datasheets/{id:guid}/derp/{otherId:int}");
    }

    private sealed record QueryParamGetRequest : ITestRequest<QueryParamGetRequest>
    {
        [QueryParam] public string Filter { get; init; } = "filterby[stuff]_asc[true]";
        public string ExpectedRoute => $"gerp-datasheets?Filter={Filter}";
        public static HttpConfiguration Configuration { get; } = HttpConfiguration.GetAnonymous("gerp-datasheets");
    }

    private sealed record RouteAndQueryParamGetRequest : ITestRequest<RouteAndQueryParamGetRequest>
    {
        [QueryParam] public string Filter { get; init; } = "filterby[stuff]_asc[true]";
        [QueryParam] public string OtherFilter { get; init; } = "filterby[mop]";
        public Guid Id { get; init; } = Guid.NewGuid();
        public int OtherId { get; init; } = 2;
        public string ExpectedRoute => $"gerp-datasheets/{Id}/derp/{OtherId}?Filter={Filter}&OtherFilter={OtherFilter}";
        public static HttpConfiguration Configuration { get; } = HttpConfiguration.GetAnonymous("gerp-datasheets/{id:guid}/derp/{otherId:int}");
    }

    private sealed record RouteAndQueryParamPostRequest : ITestRequest<RouteAndQueryParamPostRequest>
    {
        [QueryParam] public string Filter { get; init; } = "filterby[stuff]_asc[true]";
        [QueryParam] public string OtherFilter { get; init; } = "filterby[mop]";
        public Guid Id { get; init; } = Guid.NewGuid();
        public int OtherId { get; init; } = 2;
        public DateTime Created { get; init; } = DateTime.Now;
        public decimal Money { get; set; } = 3.3m;
        public string ExpectedRoute => $"gerp-datasheets/{Id}/derp/{OtherId}?Filter={Filter}&OtherFilter={OtherFilter}";
        public static HttpConfiguration Configuration { get; } = HttpConfiguration.PostAnonymous("gerp-datasheets/{id:guid}/derp/{otherId:int}");
    }

    #endregion
}