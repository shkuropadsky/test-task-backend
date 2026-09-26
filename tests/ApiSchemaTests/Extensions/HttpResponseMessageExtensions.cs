using Microsoft.OpenApi;
using Json.Schema;
using System.Text.Json;
using System.Net;

namespace ApiSchemaTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task AssertEndpointAsync(this HttpResponseMessage response,
        OpenApiDocument specDocument,
        string endpoint,
        HttpMethod method,
        HttpStatusCode statusCode,
        string mediaType)
    {
        OpenApiMediaType specMediaType = specDocument
            .Paths[endpoint]
            .Operations[method]
            .Responses[((int)statusCode).ToString()]
            .Content[mediaType];

        Assert.Equal(statusCode, response.StatusCode);
        Assert.Equal(mediaType, response.Content.Headers.ContentType?.MediaType);

        // схема контента по спецификации
        StringWriter stringWriter = new();
        specMediaType.Schema.SerializeAsV3(new OpenApiJsonWriter(stringWriter));
        JsonSchema specSchema = JsonSchema.FromText(stringWriter.ToString());

        // документ контента реального ответа по эндпоинту
        string content = await response.Content.ReadAsStringAsync();
        JsonDocument responseDoc = JsonDocument.Parse(JsonSerializer.Serialize(content));

        var results = specSchema.Evaluate(responseDoc.RootElement, new EvaluationOptions { OutputFormat = OutputFormat.List });

        Assert.True(results.IsValid, $"ERROR: {method} {endpoint}: {JsonSerializer.Serialize(results)}");
    }

}