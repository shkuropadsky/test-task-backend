using MinimalAPI.Application.Interfaces;
using MinimalAPI.Application.Models;
using MinimalAPI.Domain;

namespace MinimalAPI.Tests.Endpoints.Report;

static class ReportTestsArrange
{
    /// <summary>
    /// Демо-записи для тестов схемы API (ключи)
    /// </summary>
    public const string DEMO_PROGRESS_077 = "00000077-e090-4d18-8654-678e463b7aaa";
    public const string DEMO_PROGRESS_100 = "00000100-e090-4d18-8654-678e463b7bbb";
    public const string DEMO_USER = "b28d0ced-8af5-4c94-8650-c7946241fd1a";

    // заглушка
    private static IServiceProvider _serviceProviderMock = new EmptyServiceProvider();

    /// <summary>
    /// Добавляет в реестр демо-записи для тестирования только API:
    /// только данные, без реальных фоновых задач 
    /// </summary>
    public static void AddDemoRecords(IReportTaskRegistry taskRegistry)
    {
        StatQuery query77 = new(Guid.Parse(DEMO_PROGRESS_077), Guid.Parse(DEMO_USER), DateTime.Now, DateTime.Now, DateTime.Now);
        ReportTask task77 = new ReportTask(new StatQueryTask(_serviceProviderMock)
        {
            Query = query77,
            Percent = 77
        });
        taskRegistry.Add(query77.Id, task77);
        task77.ThreadTask = new Task(() => { });

        StatQuery query100 = new(Guid.Parse(DEMO_PROGRESS_100), Guid.Parse(DEMO_USER), DateTime.Now, DateTime.Now, DateTime.Now);
        ReportTask task100 = new ReportTask(new StatQueryTask(_serviceProviderMock)
        {
            Query = query100,
            Percent = 100,
            Result = new StatQueryResult(Guid.NewGuid(), query100.Id, StatQueryTask.DEFAULT_COUNT_SIGN_IN, DateTime.Now)
        });
        taskRegistry.Add(query100.Id, task100);
        task100.ThreadTask = new Task(() => { });
    }

    public class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

}