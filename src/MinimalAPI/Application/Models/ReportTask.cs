using MinimalAPI.Domain;

namespace MinimalAPI.Application.Models;

public class ReportTask
{
    public required StatQueryTask DomainTask { get; init; }
    
    public required Task ThreadTask { get; init; }
    
}