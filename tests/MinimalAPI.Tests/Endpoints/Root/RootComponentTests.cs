using System.Net;

namespace MinimalAPI.Tests.Endpoints.Root;

public class RootComponentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RootComponentTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRoot_ReturnsHelloWorld()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello World!", content);
    }
}