using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MinimalAPI.Application.Services;
using MinimalAPI.Contracts;
using MinimalAPI.Domain;

namespace MinimalAPI.ComponentTests.Endpoints;

public class ReportEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReportEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
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
        string url = endpoint + "?query=" + ReportTaskRegistry.DEMO_PROGRESS_077; // 77 + null
        Guid queryId = Guid.Parse(ReportTaskRegistry.DEMO_PROGRESS_077);

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
        string url = endpoint + "?query=" + ReportTaskRegistry.DEMO_PROGRESS_100; // 100 + Result
        Guid queryId = Guid.Parse(ReportTaskRegistry.DEMO_PROGRESS_100);
        Guid userId = Guid.Parse(ReportTaskRegistry.DEMO_USER);
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