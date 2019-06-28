FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Development

COPY ["./bin/output", "."]
ENTRYPOINT ["dotnet", "WebApp.dll"]
