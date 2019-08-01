FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["dllimport/dllimport.csproj", "dllimport/"]
RUN dotnet restore "dllimport/dllimport.csproj"
COPY . .
WORKDIR "/src/dllimport"
RUN dotnet build "dllimport.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "dllimport.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "dllimport.dll"]