FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
EXPOSE 12345
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["OpenTelemetrySample/OpenTelemetrySample.csproj", "OpenTelemetrySample/"]
RUN dotnet restore "OpenTelemetrySample/OpenTelemetrySample.csproj"
COPY . .
WORKDIR "/src/OpenTelemetrySample"
RUN dotnet build "OpenTelemetrySample.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OpenTelemetrySample.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OpenTelemetrySample.dll"]