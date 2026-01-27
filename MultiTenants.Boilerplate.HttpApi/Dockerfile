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
COPY ["MultiTenants.Boilerplate.sln", "."]
COPY ["MultiTenants.Boilerplate.Shared/MultiTenants.Boilerplate.Shared.csproj", "MultiTenants.Boilerplate.Shared/"]
COPY ["MultiTenants.Boilerplate.Domain/MultiTenants.Boilerplate.Domain.csproj", "MultiTenants.Boilerplate.Domain/"]
COPY ["MultiTenants.Boilerplate.Application/MultiTenants.Boilerplate.Application.csproj", "MultiTenants.Boilerplate.Application/"]
COPY ["MultiTenants.Boilerplate.HttpApi/MultiTenants.Boilerplate.HttpApi.csproj", "MultiTenants.Boilerplate.HttpApi/"]

# Restore dependencies
RUN dotnet restore "MultiTenants.Boilerplate.sln"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/MultiTenants.Boilerplate.HttpApi"
RUN dotnet build "./MultiTenants.Boilerplate.HttpApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION
RUN dotnet publish "./MultiTenants.Boilerplate.HttpApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
# Use COPY --chown for better performance instead of separate chown command
# This sets ownership during copy, avoiding an additional filesystem operation
COPY --from=publish --chown=appuser:appuser /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "MultiTenants.Boilerplate.HttpApi.dll"]
