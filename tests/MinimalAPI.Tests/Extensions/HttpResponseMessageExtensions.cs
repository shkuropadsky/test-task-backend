using Microsoft.OpenApi;
using Json.Schema;
using System.Text.Json;
using System.Net;

namespace MinimalAPI.Tests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task AssertEndpointAsync(this HttpResponseMessage response,
        OpenApiDocument specDocument,
        string url,
        HttpMethod method,
        HttpStatusCode statusCode,
        string mediaType)
    {
        string endpoint = url.Split('?')[0];

        IOpenApiResponse specApiResponse = specDocument 
            .Paths[endpoint]
            .Operations![method]
            .Responses![((int)statusCode).ToString()];
        Assert.Equal(statusCode, response.StatusCode);

        if (specApiResponse.Content == null)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(string.IsNullOrWhiteSpace(responseBody));
            return;            
        }

        OpenApiMediaType specMediaType = specApiResponse.Content![mediaType];

        Assert.Equal(statusCode, response.StatusCode);
        Assert.Equal(mediaType, response.Content.Headers.ContentType?.MediaType);

        // схема контента по спецификации
        StringWriter stringWriter = new();
        specMediaType.Schema!.SerializeAsV31(new OpenApiJsonWriter(stringWriter));
        JsonSchema specSchema = JsonSchema.FromText(stringWriter.ToString());

        // документ контента реального ответа по эндпоинту
        string content = await response.Content.ReadAsStringAsync();
        content = mediaType.Contains("json", StringComparison.OrdinalIgnoreCase) ? content : JsonSerializer.Serialize(content);
        JsonDocument responseDoc = JsonDocument.Parse(content);

        var results = specSchema.Evaluate(responseDoc.RootElement, new EvaluationOptions { OutputFormat = OutputFormat.List });

        Assert.True(results.IsValid, $"ERROR: {method} {url}: {JsonSerializer.Serialize(results)}");
    }

}