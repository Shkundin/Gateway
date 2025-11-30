# КПО-3 — Gateway + микросервисы для проверки работ на плагиат

Учебный проект по дисциплине «Конструирование программного обеспечения».

Решение построено в виде трёх микросервисов:

- **Gateway.Api** — шлюз, через который к системе обращается клиент (преподаватель / студент).
- **FileService.Api** — сервис хранения файлов студенческих работ.
- **AnalysisService.Api** — сервис анализа работ и фиксации фактов плагиата.

Все проекты написаны на **.NET 8 / ASP.NET Core Web API** и используют **SQLite** через **Entity Framework Core**.

---

## Архитектура

### Общая идея

1. Студент отправляет файл своей работы через **Gateway.Api**.
2. Gateway:
   - загружает файл в **FileService.Api**;
   - передаёт в **AnalysisService.Api** данные о работе (ID файла, студент, ID задания);
   - получает от AnalysisService результат проверки на плагиат;
   - сохраняет результат и возвращает краткий DTO клиенту.
3. Преподаватель запрашивает отчёт по конкретному заданию в **Gateway.Api** и получает список всех работ и отметку, какие из них считаются плагиатом.

> В учебной версии логика AnalysisService упрощена: плагиатом считается повторная сдача того же задания тем же студентом (связь по StudentId + AssignmentId). Это демонстрация архитектуры и взаимодействия сервисов.

---

## Структура решения

```text
Gateway.sln
├─ Gateway.Api/          # API-шлюз: загрузка работ и получение отчётов
│  ├─ Controllers/
│  │  └─ WorksController.cs
│  ├─ Data/
│  │  └─ GatewayDbContext.cs
│  ├─ Models/
│  │  ├─ Submission.cs
│  │  ├─ SubmissionUploadRequest.cs
│  │  ├─ SubmissionResponseDto.cs
│  │  └─ ReportItemDto.cs
│  └─ Program.cs
│
├─ FileService.Api/      # сервис хранения файлов
│  ├─ Controllers/
│  │  └─ FileController.cs
│  ├─ Data/
│  │  └─ FileServiceDbContext.cs
│  ├─ Models/
│  │  └─ FileRecord.cs
│  └─ Program.cs
│
└─ AnalysisService.Api/  # сервис анализа/плагиата (упрощённый мок)
   ├─ Controllers/
   │  └─ AnalysisController.cs
   ├─ Models/
   │  ├─ AnalysisRequestDto.cs
   │  └─ AnalysisResponseDto.cs
   ├─ Services/
   │  └─ TextAnalysisService.cs
   └─ Program.cs
