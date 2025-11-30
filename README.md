# КПО-3 — Gateway + микросервисы для проверки работ на плагиат

Учебный проект по дисциплине «Конструирование программного обеспечения».

Решение построено в виде трёх микросервисов:

- **Gateway.Api** — шлюз, через который к системе обращается клиент (преподаватель / студент).
- **FileService.Api** — сервис хранения файлов студенческих работ.
- **AnalysisService.Api** — сервис анализа работ и фиксации фактов плагиата (упрощённый мок).

Все проекты написаны на **.NET 8 / ASP.NET Core Web API** и используют **SQLite** через **Entity Framework Core**.

---

## Архитектура

### Общая идея

1. Студент отправляет файл своей работы через **Gateway.Api**.
2. Gateway:
   - сохраняет информацию о сдаче в своей БД;
   - в архитектурной модели предполагается взаимодействие с **FileService.Api** (хранение файла) и **AnalysisService.Api** (проверка текста на плагиат);
   - возвращает клиенту результат: статус проверки, флаг плагиата, ссылку на оригинальную работу (если найден плагиат).
3. Преподаватель запрашивает отчёт по конкретному заданию в **Gateway.Api** и получает список всех работ и отметку, какие из них считаются плагиатом.

> В учебной реализации логика проверки плагиата упрощена: плагиатом считается повторная сдача того же задания тем же студентом (связь по `StudentId` + `AssignmentId`). Это демонстрирует архитектуру и базовую бизнес-логику.

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
```

Каждый сервис — отдельный ASP.NET Core Web API-проект c собственным `Program.cs`, (где нужно) `DbContext` и Swagger-документацией.

---

## Используемые технологии

- .NET 8.0  
- ASP.NET Core Web API  
- Entity Framework Core 8 + SQLite  
- Swagger (Swashbuckle) для интерактивного тестирования API  
- Паттерн **API Gateway + микросервисы**

---

## Модель данных (упрощённо)

### Gateway.Api — таблица `Submissions`

Хранит информацию о сдачах работ:

- `Id` — int, первичный ключ  
- `AssignmentId` — int, ID задания  
- `StudentName` — string, имя студента  
- `StudentId` — string, идентификатор студента  
- `FileId` — string, идентификатор файла (GUID, который вернул FileService)  
- `Status` — string, статус проверки (`DONE` и т.п.)  
- `IsPlagiarism` — bool?, флаг плагиата  
- `PlagiarizedFromSubmissionId` — int?, ID работы-оригинала  
- `CreatedAt`, `UpdatedAt` — дата создания и обновления записи  

### FileService.Api — таблица `Files`

Хранит метаданные по загруженным файлам:

- `Id` — GUID файла (используется как внешний ключ в Gateway)  
- `FileName` — исходное имя файла  
- `ContentType` — MIME-тип  
- `StoragePath` — путь к файлу на диске  
- `CreatedAt` — дата загрузки  

### AnalysisService.Api

В учебной реализации AnalysisService не работает с БД — это мок.  
Логика анализа инкапсулирована в `TextAnalysisService` и может быть заменена на настоящую проверку в будущем (например, сравнение текстов, поиск совпадений и т.п.).

---

## Как запустить проект локально

### 1. Требования

- **Visual Studio 2022 (Community)**  
  с рабочей нагрузкой **«ASP.NET и веб-разработка»**
- **.NET SDK 8.0+**

### 2. Клонирование репозитория

```bash
git clone https://github.com/Shkundin/Gateway.git
cd Gateway
```

### 3. Открытие в Visual Studio

Откройте файл решения `Gateway.sln` в Visual Studio:

- **Файл → Открыть → Проект/Решение**

В обозревателе решений должны быть три проекта:

- `Gateway.Api`
- `FileService.Api`
- `AnalysisService.Api`

### 4. Настройка нескольких стартовых проектов

1. В обозревателе решений **ПКМ по решению** → **Свойства**.
2. В разделе **Запускаемый проект (Startup Project)** выбрать:
   - **«Несколько запускаемых проектов (Multiple startup projects)»**.
3. Для всех трёх проектов (`Gateway.Api`, `FileService.Api`, `AnalysisService.Api`) установить действие **«Запустить (Start)»**.
4. Нажать **ОК**.

### 5. Первый запуск

- Нажмите **F5** или **«Запуск без отладки»**.  
- Должны открыться три окна браузера со Swagger UI, по одному на каждый сервис.

---

## Swagger-эндпоинты

Ниже приведены ключевые эндпоинты. Порты (`7xxx`, `6xxx`, `5xxx`) могут отличаться.

---

### Gateway.Api

Базовый адрес (пример):

```text
https://localhost:7xxx/swagger/index.html
```

#### Загрузка работы студента

```http
POST /api/works/{assignmentId}/submissions
```

**Параметр пути:**

- `assignmentId` — ID задания (целое)

**Тело запроса (multipart/form-data):**

- `File` — файл работы (обязателен)
- `StudentName` — имя студента
- `StudentId` — идентификатор студента

**Пример успешного ответа (200 OK):**

```json
{
  "submissionId": 1,
  "status": "DONE",
  "isPlagiarism": false,
  "plagiarizedFromSubmissionId": null
}
```

Если тот же студент повторно сдаёт то же задание:

```json
{
  "submissionId": 2,
  "status": "DONE",
  "isPlagiarism": true,
  "plagiarizedFromSubmissionId": 1
}
```

#### Получение отчёта по заданию

```http
GET /api/works/{assignmentId}/reports
```

**Пример ответа:**

```json
{
  "assignmentId": 1,
  "reports": [
    {
      "submissionId": 1,
      "studentName": "Иван Иванов",
      "studentId": "st_001",
      "status": "DONE",
      "isPlagiarism": false,
      "plagiarizedFromSubmissionId": null
    },
    {
      "submissionId": 2,
      "studentName": "Иван Иванов",
      "studentId": "st_001",
      "status": "DONE",
      "isPlagiarism": true,
      "plagiarizedFromSubmissionId": 1
    }
  ]
}
```

---

### FileService.Api

Базовый адрес (пример):

```text
https://localhost:6xxx/swagger/index.html
```

#### Загрузка файла

```http
POST /api/File
```

**Параметры:**

- `file` — файл (формат `IFormFile`)

**Пример ответа:**

```json
{
  "fileId": "d3c61b85-74a0-4c3b-97f1-5bca981e7b3f",
  "fileName": "work1.txt"
}
```

#### Получение файла

```http
GET /api/File/{id}
```

**Параметр пути:**

- `id` — GUID файла, который вернулся при загрузке.

Возвращает содержимое файла с корректным `Content-Type`.

---

### AnalysisService.Api

Базовый адрес (пример):

```text
https://localhost:5xxx/swagger/index.html
```

#### Мок-эндпоинт для анализа текста

```http
POST /api/Analysis
```

**Тело запроса:**

```json
{
  "text": "какой-то текст работы"
}
```

**Пример ответа:**

```json
{
  "isPlagiarism": false,
  "similarityScore": 0.0
}
```

В учебной реализации всегда возвращается «не плагиат», реальная логика могла бы сравнивать хеши или использовать алгоритмы анализа текста.

---

## Пример сценария использования (через Swagger)

1. Запустить все три сервиса.
2. Открыть Swagger **Gateway.Api**.
3. В разделе **Works** вызвать:

   ```http
   POST /api/works/{assignmentId}/submissions
   ```

   с `assignmentId = 1` и данными студента.

4. Ещё раз вызвать тот же метод для того же `StudentId` и `assignmentId` (можно с тем же файлом).
5. Вызвать:
## Запуск через Docker

```bash
docker compose up --build

   ```http
   GET /api/works/1/reports
   ```

   - первая запись — оригинал (`isPlagiarism: false`);
   - вторая запись — плагиат (`isPlagiarism: true`, `plagiarizedFromSubmissionId` — ID первой работы).

---

## Дальнейшее развитие

- Реализовать реальное взаимодействие `Gateway → FileService → AnalysisService` (через HTTP-клиенты, а не через мок).
- Вынести строки подключения в `appsettings.json` всех сервисов.
- Добавить аутентификацию и авторизацию (роли «преподаватель» / «студент»).
- Заменить примитивную логику анализа на настоящий алгоритм поиска плагиата.

---

## Лицензия

Проект создан в учебных целях в рамках курса по КПО.  
Условия повторного использования и копирования зависят от требований преподавателя/вуза.
