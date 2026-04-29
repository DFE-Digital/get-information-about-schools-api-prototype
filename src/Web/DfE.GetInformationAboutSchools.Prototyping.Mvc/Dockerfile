# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution + restore
COPY *.sln .
COPY src/Web/DfE.GetInformationAboutSchools.Prototyping.Mvc/*.csproj ./src/Web/DfE.GetInformationAboutSchools.Prototyping.Mvc/
RUN dotnet restore


# Copy everything and build
COPY . .
RUN dotnet publish src/Web/DfE.GetInformationAboutSchools.Prototyping.Mvc/DfE.GetInformationAboutSchools.Prototyping.Mvc.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose port (match the appsettings or Program.cs)
EXPOSE 8080

# Run the MVC app
ENTRYPOINT ["dotnet", "DfE.GetInformationAboutSchools.Prototyping.Mvc.dll"]
