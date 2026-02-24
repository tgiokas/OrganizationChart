FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/API/OrganizationChart.Api.csproj", "src/API/"]
COPY ["src/Application/OrganizationChart.Application.csproj", "src/Application/"]
COPY ["src/Domain/OrganizationChart.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/OrganizationChart.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "./src/API/OrganizationChart.Api.csproj"
COPY . .
WORKDIR "src/API"
RUN dotnet build "./OrganizationChart.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrganizationChart.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OrganizationChart.Api.dll"]
