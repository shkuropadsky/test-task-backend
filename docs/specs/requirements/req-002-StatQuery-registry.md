# REQ-002. StatQuery registry
---
Полученный POST-запрос должен регистрироваться для последующей проверки статуса GET-запросом.
И зарегистрированных запросов, и проверок их статуса одновременно может быть много.

### Примечания
Требование к сохранности запросов при перезагрузке приложения пока не рассматривается.


### curl
curl http://localhost:5155/report/info?query=1a98b57d-e090-4d18-8654-678e463b7aaa


### Invoke-RestMethod
#### GET
Invoke-RestMethod -Uri "http://localhost:5155/report/info?query=1a98b57d-e090-4d18-8654-678e463b7aaa"

irm "http://localhost:5155/report/info?query=1a98b57d-e090-4d18-8654-678e463b7aaa"

iwr "http://localhost:5155/report/info?query=1a98b57d-e090-4d18-8654-678e463b7bbb"

#### POST
Invoke-RestMethod -Uri "http://localhost:5155/report/user_statistics" `
                  -Method Post `
                  -ContentType "application/json" `
                  -Body '{"user_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "from": "2026-09-01T00:00:00Z", "to": "2026-09-30T23:59:59Z"}'

irm "http://localhost:5155/report/user_statistics" -Method Post -ContentType "application/json" -Body '{"user_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "from": "2026-09-01T00:00:00Z", "to": "2026-09-30T23:59:59Z"}'

iwr "http://localhost:5155/report/user_statistics" -Method Post -ContentType "application/json" -Body '{"user_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "from": "2026-09-01T00:00:00Z", "to": "2026-09-30T23:59:59Z"}'

