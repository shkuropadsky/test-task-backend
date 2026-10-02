using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ApiSchemaTests.Extensions;
using Microsoft.OpenApi;
using MinimalAPI.Application.Services;
using MinimalAPI.Contracts;

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
    public async Task ReportUserStatisticsEndpoint_Matches_OpenApiSpec()
    {
        string userId = "1a98b57d-e090-4d18-8654-678e463b7aaa";

        var requestDto = new ReportUserStatisticsRequestDto(
            UserId: Guid.Parse(userId),
            From: DateTime.UtcNow.AddDays(-7),
            To: DateTime.UtcNow
        );

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        string endpoint = "/report/user_statistics";
        string url = endpoint;

        HttpResponseMessage response = await _client.PostAsJsonAsync(url, requestDto, jsonOptions);

        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Post, HttpStatusCode.OK, "text/plain");
    }


    [Fact]
    public async Task ReportInfoEndpoint_Matches_OpenApiSpec_HasNoResult()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + ReportTaskRegistry.DEMO_PROGRESS_077; // 77 + null

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "application/json");
    }

    [Fact]
    public async Task ReportInfoEndpoint_Matches_OpenApiSpec_HasResult()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + ReportTaskRegistry.DEMO_PROGRESS_100; // 100 + Result

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "application/json");
    }
}