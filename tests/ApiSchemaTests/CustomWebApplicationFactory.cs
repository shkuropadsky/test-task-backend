using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace ApiSchemaTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public required OpenApiDocument OpenApiDoc { get; init; }

    public CustomWebApplicationFactory()
    {
        string yamlPath = Path.Combine(@"..\..\..\..\..", @"docs\specs\api", "openapi.yaml");
        string yamlContent = File.ReadAllText(yamlPath);

        var settings = new OpenApiReaderSettings();
        settings.AddYamlReader();
        var parseResult = OpenApiDocument.Parse(yamlContent, "yaml", settings);

        OpenApiDoc = parseResult.Document ?? 
            throw new InvalidOperationException($"Ошибка парсинга OpenAPI спецификации: {yamlPath}");;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}