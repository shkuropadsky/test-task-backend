using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.OpenApi;
using MinimalAPI.Contracts;
using MinimalAPI.Tests.Extensions;

namespace MinimalAPI.Tests.Endpoints.Report;

public class ReportSchemaTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly OpenApiDocument _doc;

    public ReportSchemaTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _doc = factory.OpenApiDoc;
    }

    [Fact]
    public async Task PostReportUserStatistics_MatchesApiSpec()
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
    public async Task GetReportInfo_NewGuid_MatchesApiSpec()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + Guid.NewGuid(); // новый - как заведомо отсутствующий

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.NotFound, "application/json");
    }

    [Fact]
    public async Task GetReportInfo_DemoProgress077_MatchesApiSpec()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + ReportTestsArrange.DEMO_PROGRESS_077; // 77 + null

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "application/json");
    }

    [Fact]
    public async Task GetReportInfo_DemoProgress100_MatchesApiSpec()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + ReportTestsArrange.DEMO_PROGRESS_100; // 100 + Result

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "application/json");
    }
}