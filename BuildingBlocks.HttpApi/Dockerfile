ARG DOTNET_VERSION=10.0
ARG APP_UID=10000
ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
ARG APP_UID
RUN groupadd -r appuser && useradd -r -u ${APP_UID} -g appuser appuser
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG BUILD_CONFIGURATION
WORKDIR /src

# Copy solution and project files
COPY ["BuildingBlocks.sln", "."]
COPY ["BuildingBlocks.Shared/BuildingBlocks.Shared.csproj", "BuildingBlocks.Shared/"]
COPY ["BuildingBlocks.Domain/BuildingBlocks.Domain.csproj", "BuildingBlocks.Domain/"]
COPY ["BuildingBlocks.Application/BuildingBlocks.Application.csproj", "BuildingBlocks.Application/"]
COPY ["BuildingBlocks.HttpApi/BuildingBlocks.HttpApi.csproj", "BuildingBlocks.HttpApi/"]

# Restore dependencies
RUN dotnet restore "BuildingBlocks.sln"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/BuildingBlocks.HttpApi"
RUN dotnet build "./BuildingBlocks.HttpApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION
RUN dotnet publish "./BuildingBlocks.HttpApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
# Use COPY --chown for better performance instead of separate chown command
# This sets ownership during copy, avoiding an additional filesystem operation
COPY --from=publish --chown=appuser:appuser /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "BuildingBlocks.HttpApi.dll"]
