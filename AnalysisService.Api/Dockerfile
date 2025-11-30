FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AnalysisService.Api/AnalysisService.Api.csproj", "AnalysisService.Api/"]
COPY ["Gateway.Api/Gateway.Api.csproj", "Gateway.Api/"]
COPY ["FileService.Api/FileService.Api.csproj", "FileService.Api/"]

RUN dotnet restore "AnalysisService.Api/AnalysisService.Api.csproj"

COPY . .

WORKDIR "/src/AnalysisService.Api"
RUN dotnet build "AnalysisService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnalysisService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AnalysisService.Api.dll"]
