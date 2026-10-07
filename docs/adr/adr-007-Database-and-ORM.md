# ADR-007. Database and ORM
## Контекст
Требования задания:
> Если приложение перезагрузить информацию о запросе не должна быть потеряна.
> Желательно использовать ORM, структуру базы данных делать через миграции.
## Решение
1. Database: PostgreSQL (`docker-compose.yaml`)
2. ORM: EntityFramework (поддержка миграций)
## Последствия
Если выполнение задачи было прервано, то при обращении по 'QueryId' не генерируется ошибка, а задача просто начинает выполняться заново.

### Migrations
dotnet ef migrations add ... --context PostgresReportDbContext --output-dir DataStorage/Postgres/Migrations
dotnet ef database update --context PostgresReportDbContext