using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using MinimalAPI.Contracts;
using MinimalAPI.Domain;

namespace MinimalAPI.Tests.Endpoints.Report;

public class ReportIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    const string USER_ID = "b28d0ced-8af5-4c94-8650-c7946241fd1a";
    const int DELAY = 60000; // см. `appsettings.json` основного проекта

    static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true
    };

    private readonly HttpClient _client;

    public ReportIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostQuery_DontWait_GetPercent0()
    {
        Console.WriteLine($"==== GET 0 ====");

        // 1. POST
        Guid queryId = await PostQuery();

        // NO WAITING

        // 2. GET        
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + queryId;

        HttpResponseMessage response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ReportInfoResponseDto>(_jsonOptions);
        string str = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GET response.Content (JSON) = {JsonNode.Parse(str)?.ToJsonString(_jsonOptions)}");

        Assert.Equal(queryId, dto?.Query);
        Assert.Equal(0, dto?.Percent);
        Assert.Null(dto?.Result);

        Console.WriteLine($".... G0T 0 ....");
    }

    [Fact]
    public async Task PostQuery_WaitHalf_GetPercent50()
    {
        Console.WriteLine($"==== GET 50 ====");

        // 1. POST
        Guid queryId = await PostQuery();

        // WAIT
        Console.WriteLine($"---- GO SLEEP 50 ----");
        Thread.Sleep(DELAY / 2);
        Console.WriteLine($"---- WAKE UP 50 ----");

        // 2. GET        
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + queryId;

        HttpResponseMessage response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ReportInfoResponseDto>(_jsonOptions);
        string str = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GET response.Content (JSON) = {JsonNode.Parse(str)?.ToJsonString(_jsonOptions)}");

        Assert.Equal(queryId, dto?.Query);
        Assert.Equal(50, dto?.Percent);
        Assert.Null(dto?.Result);

        Console.WriteLine($".... G0T 50 ....");
    }

    [Fact]
    public async Task PostQuery_WaitFull_GetPercent100()
    {
        Console.WriteLine($"==== GET 100 ====");

        // 1. POST
        Guid queryId = await PostQuery();

        // WAIT
        Console.WriteLine($"---- GO SLEEP 100 ----");
        Thread.Sleep(DELAY);
        Console.WriteLine($"---- WAKE UP 100 ----");

        // 2. GET        
        string endpoint = "/report/info";
        string url = endpoint + "?query=" + queryId;

        HttpResponseMessage response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ReportInfoResponseDto>(_jsonOptions);
        string str = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GET response.Content (JSON) = {JsonNode.Parse(str)?.ToJsonString(_jsonOptions)}");

        Assert.Equal(queryId, dto?.Query);
        Assert.Equal(100, dto?.Percent);
        Assert.NotNull(dto?.Result);

        Assert.Equal(USER_ID, dto?.Result.UserId.ToString());
        string сountSignIn = StatQueryTask.DEFAULT_COUNT_SIGN_IN.ToString();
        Assert.Equal(сountSignIn, dto?.Result.CountSignIn);

        Console.WriteLine($".... G0T 100 ....");
    }

    /// <summary>
    /// Отправка нового запроса
    /// </summary>
    private async Task<Guid> PostQuery()
    {
        var requestDto = new ReportUserStatisticsRequestDto(
            UserId: Guid.Parse(USER_ID),
            From: DateTime.UtcNow.AddDays(-7),
            To: DateTime.UtcNow
        );

        string endpoint = "/report/user_statistics";
        string url = endpoint;

        HttpResponseMessage response = await _client.PostAsJsonAsync(url, requestDto, _jsonOptions);
        string str = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"POST response.Content (GUID) = {str}"); // text/plain
        Guid queryId = Guid.Parse(str);
        return queryId;
    }
}