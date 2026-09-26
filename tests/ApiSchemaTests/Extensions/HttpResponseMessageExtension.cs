using Microsoft.OpenApi.Models;
using Json.Schema;
using System.Text.Json;
using System.Net;
using Microsoft.OpenApi.Writers;

namespace ApiSchemaTests.Extensions;

public static class OpenApiAssert
{
    public static async Task AssertSchemaAsync(this HttpResponseMessage response,
        OpenApiDocument doc,
        string endpoint,
        OperationType method,
        HttpStatusCode statusCode,
        string mediaType)
    {
        var spec = doc
            .Paths[endpoint]
            .Operations[method]
            .Responses[((int)statusCode).ToString()]
            .Content[mediaType];

        Assert.Equal(statusCode, response.StatusCode);
        Assert.Equal(mediaType, response.Content.Headers.ContentType?.MediaType);

        StringWriter stringWriter = new();
        spec.Schema.SerializeAsV3(new OpenApiJsonWriter(stringWriter));
        var specJson = JsonSchema.FromText(stringWriter.ToString());

        string content = await response.Content.ReadAsStringAsync();
        var contentJson = JsonDocument.Parse(JsonSerializer.Serialize(content));

        var results = specJson.Evaluate(contentJson.RootElement, new EvaluationOptions { OutputFormat = OutputFormat.List });

        Assert.True(results.IsValid, $"ERROR: {method} {endpoint}: {JsonSerializer.Serialize(results)}");
    }
}