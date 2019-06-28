FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app

COPY ["./bin/output", "."]
ENTRYPOINT ["dotnet", "DockerComposeDnsResolution.dll", "lookup"]
