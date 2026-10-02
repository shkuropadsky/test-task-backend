using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ApiSchemaTests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;
using MinimalAPI.Application.Services;
using MinimalAPI.Contracts;
using MinimalAPI.Domain;

namespace ApiSchemaTests.Endpoints;

public class ReportEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly OpenApiDocument _doc;

    private IReportTaskRegistry _taskRegistry;

    // заглушка
    private IServiceProvider _serviceProviderMock = new EmptyServiceProvider();

    public class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }


    public ReportEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _doc = factory.OpenApiDoc;

        _taskRegistry = factory.Services.GetRequiredService<IReportTaskRegistry>(); ;

        // Демо-записи для тестов схемы API (объекты-заглушки)
        AddDemoRecords();
    }

    /// <summary>
    /// Демо-записи для тестов схемы API (ключи)
    /// </summary>
    public const string DEMO_PROGRESS_077 = "00000077-e090-4d18-8654-678e463b7aaa";
    public const string DEMO_PROGRESS_100 = "00000100-e090-4d18-8654-678e463b7bbb";
    public const string DEMO_USER = "b28d0ced-8af5-4c94-8650-c7946241fd1a";


    /// <summary>
    /// Добавляет в реестр демо-записи для тестирования только API:
    /// только данные, без реальных фоновых задач 
    /// </summary>
    private void AddDemoRecords()
    {
        StatQuery query77 = new(Guid.Parse(DEMO_PROGRESS_077), Guid.Parse(DEMO_USER), DateTime.Now, DateTime.Now);
        _taskRegistry.Add(query77.Id, new ReportTask()
        {
            DomainTask = new StatQueryTask(_serviceProviderMock)
            {
                Query = query77,
                Percent = 77
            },
            ThreadTask = new Task(() => { })
        });

        StatQuery query100 = new(Guid.Parse(DEMO_PROGRESS_100), Guid.Parse(DEMO_USER), DateTime.Now, DateTime.Now);
        _taskRegistry.Add(query100.Id, new ReportTask()
        {
            DomainTask = new StatQueryTask(_serviceProviderMock)
            {
                Query = query100,
                Percent = 100,
                Result = new StatQueryResult(StatQueryTask.DEFAULT_COUNT_SIGN_IN)
            },
            ThreadTask = new Task(() => { })
        });
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
        string url = endpoint + "?query=" + DEMO_PROGRESS_077; // 77 + null

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "application/json");
    }

    [Fact]
    public async Task ReportInfoEndpoint_Matches_OpenApiSpec_HasResult()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + DEMO_PROGRESS_100; // 100 + Result

        HttpResponseMessage response = await _client.GetAsync(url);
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        await response.AssertEndpointAsync(_doc, url, HttpMethod.Get, HttpStatusCode.OK, "application/json");
    }
}