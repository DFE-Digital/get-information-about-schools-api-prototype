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
ARG GITHUB_TOKEN

WORKDIR /src

# Copy NuGet.config into the build context AND into the correct NuGet folder
COPY ["NuGet.config", "."]
COPY NuGet.config /root/.nuget/NuGet/NuGet.Config

# Copy project files
COPY ["src/API/DfE.GetInformationAboutSchools.Prototyping.API.csproj", "src/API/"]
COPY ["src/Core/DfE.GetInformationAboutSchools.Prototyping.Core.csproj", "src/Core/"]
COPY ["src/Infrastructure/DfE.GetInformationAboutSchools.Prototyping.Infrastructure.csproj", "src/Infrastructure/"]

# Restore using authenticated NuGet.config
RUN dotnet restore "./src/API/DfE.GetInformationAboutSchools.Prototyping.API.csproj"

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
