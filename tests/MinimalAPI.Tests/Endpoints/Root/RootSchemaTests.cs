using System.Net;
using Microsoft.OpenApi;
using MinimalAPI.Tests.Extensions;

namespace MinimalAPI.Tests.Endpoints.Root;

public class RootSchemaTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly OpenApiDocument _doc;
    public RootSchemaTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _doc = factory.OpenApiDoc;
    }

    [Fact]
    public async Task GetRoot_Matches_ApiSpec()
    {
        string endpoint = "/";
        string url = endpoint;

        HttpResponseMessage response = await _client.GetAsync(endpoint);
        
        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "text/plain");
    }
}