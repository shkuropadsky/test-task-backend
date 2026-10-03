using MinimalAPI.Application.Interfaces;
using MinimalAPI.Domain;

namespace MinimalAPI.DataStorage;

public class ReportStorageMemory : IReportStorage
{
    public void AddQuery(StatQuery statQuery)
    {
        //throw new NotImplementedException();
    }

    public void AddQueryResult(StatQueryResult? result)
    {
        //throw new NotImplementedException();
    }

    public StatQuery GetQuery(Guid queryId)
    {
        return null;
        //throw new NotImplementedException();
    }

    public StatQueryResult GetQueryResult(Guid queryId)
    {
        return null;
        //throw new NotImplementedException();
    }
}