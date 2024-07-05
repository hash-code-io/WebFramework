using Aisto.SharedKernel.Requests;

namespace Aisto.Test.UnitTests.Requests;

public sealed class RouteTemplateParserTests
{
    [Theory]
    [InlineData("gerp", "gerp")]
    [InlineData("gerp/{id:guid}", "gerp/{0}", "id")]
    [InlineData("gerp/{Id:guid}", "gerp/{0}", "id")]
    [InlineData("gerp/{id:guid}/", "gerp/{0}/", "id")]
    [InlineData("gerp/{id :guid}/", "gerp/{0}/", "id")]
    [InlineData("gerp/{id: guid}/", "gerp/{0}/", "id")]
    [InlineData("gerp/{id : guid}/", "gerp/{0}/", "id")]
    [InlineData("gerp/{id:guid}/other", "gerp/{0}/other", "id")]
    [InlineData("gerp/{id:guid}/other/{otherId:int}/", "gerp/{0}/other/{1}/", "id", "otherid")]
    public void Should_Parse_Valid_Template(string routeTemplate, string expectedReplacementRoute, params string[] orderedParamNames)
    {
        // Act
        Replacements result = RouteTemplateParser.Parse(routeTemplate);

        // Assert
        Assert.Equal(expectedReplacementRoute, result.ReplacementString);
        Assert.Equal(orderedParamNames, result.OrderedParamNames);
    }

    [Theory]
    [InlineData("")]
    [InlineData("gerp/{}")]
    [InlineData("gerp/{}/")]
    [InlineData("gerp/{")]
    [InlineData("gerp/}")]
    [InlineData("gerp/{id:int}/{")]
    [InlineData("gerp/{id:int}/}")]
    [InlineData("gerp/{id:int}}")]
    [InlineData("gerp/{{id:int}")]
    [InlineData("gerp/{{id:int}}")]
    public void Should_Throw_On_Invalid_Template(string routeTemplate) =>
        // Act && Assert
        Assert.Throws<InvalidOperationException>(() => RouteTemplateParser.Parse(routeTemplate));
}