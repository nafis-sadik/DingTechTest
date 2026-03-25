# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY src/DingTechTest.slnx ./src/
COPY src/Application.API/Application.API.csproj ./src/Application.API/
COPY src/Application.Domain/Application.Domain.csproj ./src/Application.Domain/
COPY src/Application.Entities/Application.Entities.csproj ./src/Application.Entities/
COPY src/System.Core/System.Core.csproj ./src/System.Core/

# Restore dependencies
RUN dotnet restore ./src/Application.API/Application.API.csproj

# Copy the remaining project files
COPY src/ ./src/

# Build and publish
RUN dotnet publish ./src/Application.API/Application.API.csproj -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port and start the application
EXPOSE 8080
ENTRYPOINT ["dotnet", "Application.API.dll"]
