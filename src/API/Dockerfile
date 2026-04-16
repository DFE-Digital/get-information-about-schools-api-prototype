# syntax=docker/dockerfile:1.6
# See https://aka.ms/customizecontainer to learn how to customize your debug container 
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# ============================================================
# Base runtime image
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ============================================================
# Build stage
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GITHUB_USERNAME

WORKDIR /src

# Copy project files
COPY ["src/API/DfE.GetInformationAboutSchools.Prototyping.API.csproj", "src/API/"]
COPY ["src/Core/DfE.GetInformationAboutSchools.Prototyping.Core.csproj", "src/Core/"]
COPY ["src/Infrastructure/DfE.GetInformationAboutSchools.Prototyping.Infrastructure.csproj", "src/Infrastructure/"]


# Restore using BuildKit secret for private package feeds
RUN --mount=type=secret,id=github_token \
    GITHUB_TOKEN="$(cat /run/secrets/github_token)" && \
    dotnet nuget remove source github 2>/dev/null || true && \
    dotnet nuget add source "https://nuget.pkg.github.com/DFE-Digital/index.json" \
      --name github \
      --username "${GITHUB_USERNAME:-x-access-token}" \
      --password "$GITHUB_TOKEN" \
      --store-password-in-clear-text && \
    dotnet restore "./src/API/DfE.GetInformationAboutSchools.Prototyping.API.csproj" && \
    dotnet nuget remove source github

# Copy the rest of the source
COPY . .

WORKDIR "/src/src/API"

# Build
RUN dotnet build "./DfE.GetInformationAboutSchools.Prototyping.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ============================================================
# Publish stage
# ============================================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DfE.GetInformationAboutSchools.Prototyping.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ============================================================
# Final runtime image
# ============================================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DfE.GetInformationAboutSchools.Prototyping.API.dll"]
