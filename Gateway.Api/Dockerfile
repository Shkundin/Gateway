# ---------------- Базовый runtime-образ ----------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Порт внутри контейнера (на него будет слушать Kestrel)
EXPOSE 8080

# ---------------- Сборочный образ (SDK) ----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только csproj, чтобы быстрее было делать restore
COPY ["Gateway.Api/Gateway.Api.csproj", "Gateway.Api/"]
COPY ["FileService.Api/FileService.Api.csproj", "FileService.Api/"]
COPY ["AnalysisService.Api/AnalysisService.Api.csproj", "AnalysisService.Api/"]

# Восстанавливаем зависимости
RUN dotnet restore "Gateway.Api/Gateway.Api.csproj"

# Копируем весь репозиторий внутрь контейнера
COPY . .

# Собираем проект в режиме Release
WORKDIR "/src/Gateway.Api"
RUN dotnet build "Gateway.Api.csproj" -c Release -o /app/build

# ---------------- Публикация ----------------
FROM build AS publish
RUN dotnet publish "Gateway.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------------- Финальный образ ----------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Запускаем приложение
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
