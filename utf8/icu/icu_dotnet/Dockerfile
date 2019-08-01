FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["icu_dotnet/icu_dotnet.csproj", "icu_dotnet/"]
RUN dotnet restore "icu_dotnet/icu_dotnet.csproj"
COPY . .
WORKDIR "/src/icu_dotnet"
RUN dotnet build "icu_dotnet.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "icu_dotnet.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "icu_dotnet.dll"]