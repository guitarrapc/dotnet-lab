FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["WebApplicationEF/WebApplicationEF.csproj", "WebApplicationEF/"]
RUN dotnet restore "WebApplicationEF/WebApplicationEF.csproj"
COPY . .
WORKDIR "/src/WebApplicationEF"
RUN dotnet build "WebApplicationEF.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebApplicationEF.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApplicationEF.dll"]