using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace ApiSchemaTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public required OpenApiDocument OpenApiDoc { get; init; }

    public CustomWebApplicationFactory()
    {
        string yamlPath = Path.Combine(@"..\..\..\..\..", @"docs\specs\api", "openapi.yaml");
        Console.WriteLine($"yamlPath = {yamlPath}");
        
        string yamlContent = File.ReadAllText(yamlPath);
        OpenApiDoc = new OpenApiStringReader().Read(yamlContent, out var diagnostic);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}