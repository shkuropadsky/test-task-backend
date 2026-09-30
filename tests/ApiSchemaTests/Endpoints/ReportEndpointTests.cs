using System.Net;
using ApiSchemaTests.Extensions;
using Microsoft.OpenApi;

namespace ApiSchemaTests.Endpoints;

public class ReportEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly OpenApiDocument _doc;
    public ReportEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _doc = factory.OpenApiDoc;
    }

    [Fact]
    public async Task ReportEndpoint_Matches_OpenApiSpec_HasNoResult()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=1a98b57d-e090-4d18-8654-678e463b7aaa"; // 77 + null

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "application/json");
    }

    [Fact]
    public async Task ReportEndpoint_Matches_OpenApiSpec_HasResult()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=1a98b57d-e090-4d18-8654-678e463b7bbb"; // 100 + Result

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "application/json");
    }
}