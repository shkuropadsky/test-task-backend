using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;
using MinimalAPI.Application.Services;
using MinimalAPI.Contracts;
using MinimalAPI.Domain;

namespace MinimalAPI.ComponentTests.Endpoints;


public class EmptyServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType) => null;
}

public class ReportEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private IReportTaskRegistry _taskRegistry;

    // заглушка
    private IServiceProvider _serviceProviderMock = new EmptyServiceProvider();

    public ReportEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
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
    public async Task GetReportInfo_NewGuid_ReturnsNotFound()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + Guid.NewGuid(); // новый - как заведомо отсутствующий

        HttpResponseMessage response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetReportInfo_DemoGuid77_ReturnsOnlyProgress()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + DEMO_PROGRESS_077; // 77 + null
        Guid queryId = Guid.Parse(DEMO_PROGRESS_077);

        HttpResponseMessage response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseDto = await response.Content.ReadFromJsonAsync<ReportInfoResponseDto>();

        Assert.NotNull(responseDto);
        Assert.Equal(queryId, responseDto.Query);
        Assert.True(responseDto.Percent == 77);

        Assert.Null(responseDto.Result);
    }

    [Fact]
    public async Task GetReportInfo_DemoGuid100_ReturnsResult()
    {
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + DEMO_PROGRESS_100; // 100 + Result
        Guid queryId = Guid.Parse(DEMO_PROGRESS_100);
        Guid userId = Guid.Parse(DEMO_USER);
        string сountSignIn = StatQueryTask.DEFAULT_COUNT_SIGN_IN.ToString();

        HttpResponseMessage response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var responseDto = await response.Content.ReadFromJsonAsync<ReportInfoResponseDto>(jsonOptions);

        Assert.NotNull(responseDto);
        Assert.Equal(queryId, responseDto.Query);
        Assert.True(responseDto.Percent == 100);

        Assert.NotNull(responseDto.Result);
        Assert.Equal(userId, responseDto.Result.UserId);
        Assert.Equal(сountSignIn, responseDto.Result.CountSignIn);
    }

}