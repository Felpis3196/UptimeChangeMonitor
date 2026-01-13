FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/UptimeChangeMonitor.API/UptimeChangeMonitor.API.csproj", "src/UptimeChangeMonitor.API/"]
COPY ["src/UptimeChangeMonitor.Application/UptimeChangeMonitor.Application.csproj", "src/UptimeChangeMonitor.Application/"]
COPY ["src/UptimeChangeMonitor.Domain/UptimeChangeMonitor.Domain.csproj", "src/UptimeChangeMonitor.Domain/"]
COPY ["src/UptimeChangeMonitor.Infrastructure/UptimeChangeMonitor.Infrastructure.csproj", "src/UptimeChangeMonitor.Infrastructure/"]

RUN dotnet restore "src/UptimeChangeMonitor.API/UptimeChangeMonitor.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/UptimeChangeMonitor.API"
RUN dotnet build "UptimeChangeMonitor.API.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/src/UptimeChangeMonitor.API"
RUN dotnet publish "UptimeChangeMonitor.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UptimeChangeMonitor.API.dll"]
