# syntax=docker/dockerfile:1.6

# ============================================================
# Base runtime image
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# ============================================================
# Build stage
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy only the MVC project file first
COPY ["src/Web/DfE.GetInformationAboutSchools.Prototyping.Mvc/DfE.GetInformationAboutSchools.Prototyping.Mvc.csproj", "src/Web/DfE.GetInformationAboutSchools.Prototyping.Mvc/"]

# Restore dependencies
RUN dotnet restore "src/Web/DfE.GetInformationAboutSchools.Prototyping.Mvc/DfE.GetInformationAboutSchools.Prototyping.Mvc.csproj"

# Copy the rest of the solution
COPY . .

# Build the MVC project
WORKDIR "/src/src/Web/DfE.GetInformationAboutSchools.Prototyping.Mvc"
RUN dotnet build "DfE.GetInformationAboutSchools.Prototyping.Mvc.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ============================================================
# Publish stage
# ============================================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DfE.GetInformationAboutSchools.Prototyping.Mvc.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ============================================================
# Final runtime image
# ============================================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DfE.GetInformationAboutSchools.Prototyping.Mvc.dll"]
