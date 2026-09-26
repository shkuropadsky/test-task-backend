using System.Net;
using ApiSchemaTests.Extensions;
using Microsoft.OpenApi.Models;

namespace ApiSchemaTests.Endpoints;

public class RootEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly OpenApiDocument _doc;
    public RootEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _doc = factory.OpenApiDoc;
    }

    [Fact]
    public async Task RootEndpoint_MustMatch_OpenApiSpecification()
    {
        HttpResponseMessage response = await _client.GetAsync("/");
        
        await response.AssertSchemaAsync(_doc, "/", OperationType.Get, HttpStatusCode.OK, "text/plain");
    }
}