FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FileService.Api/FileService.Api.csproj", "FileService.Api/"]
COPY ["Gateway.Api/Gateway.Api.csproj", "Gateway.Api/"]
COPY ["AnalysisService.Api/AnalysisService.Api.csproj", "AnalysisService.Api/"]

RUN dotnet restore "FileService.Api/FileService.Api.csproj"

COPY . .

WORKDIR "/src/FileService.Api"
RUN dotnet build "FileService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "FileService.Api.dll"]
