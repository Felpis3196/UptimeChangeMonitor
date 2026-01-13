FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/UptimeChangeMonitor.Workers/UptimeChangeMonitor.Workers.csproj", "src/UptimeChangeMonitor.Workers/"]
COPY ["src/UptimeChangeMonitor.Application/UptimeChangeMonitor.Application.csproj", "src/UptimeChangeMonitor.Application/"]
COPY ["src/UptimeChangeMonitor.Domain/UptimeChangeMonitor.Domain.csproj", "src/UptimeChangeMonitor.Domain/"]
COPY ["src/UptimeChangeMonitor.Infrastructure/UptimeChangeMonitor.Infrastructure.csproj", "src/UptimeChangeMonitor.Infrastructure/"]

RUN dotnet restore "src/UptimeChangeMonitor.Workers/UptimeChangeMonitor.Workers.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/UptimeChangeMonitor.Workers"
RUN dotnet build "UptimeChangeMonitor.Workers.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/src/UptimeChangeMonitor.Workers"
RUN dotnet publish "UptimeChangeMonitor.Workers.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UptimeChangeMonitor.Workers.dll"]
