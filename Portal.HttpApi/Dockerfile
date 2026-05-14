ARG DOTNET_VERSION=10.0
ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG BUILD_CONFIGURATION
WORKDIR /src

COPY ["MultiTenants.Boilerplate.sln", "."]
COPY ["BuildingBlocks.Core/BuildingBlocks.Core.csproj", "BuildingBlocks.Core/"]
COPY ["BuildingBlocks.Shared/BuildingBlocks.Shared.csproj", "BuildingBlocks.Shared/"]
COPY ["Identity.Domain/Identity.Domain.csproj", "Identity.Domain/"]
COPY ["Identity.Application/Identity.Application.csproj", "Identity.Application/"]
COPY ["Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity.Infrastructure/"]
COPY ["Tenancy.Domain/Tenancy.Domain.csproj", "Tenancy.Domain/"]
COPY ["Portal.HttpApi/Portal.HttpApi.csproj", "Portal.HttpApi/"]

RUN dotnet restore "Portal.HttpApi/Portal.HttpApi.csproj"

COPY . .
WORKDIR "/src/Portal.HttpApi"
RUN dotnet publish "Portal.HttpApi.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Portal.HttpApi.dll"]
