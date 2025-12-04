# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all source code into the build context
# This is required because 'dotnet restore' needs access to all project files.
COPY . .

# Restore dependencies for all projects
# If a Solution (.sln) file exists, dotnet restore will automatically use it.
RUN dotnet restore

# Build and Publish the API project (the entry point)
# The application starts from the API layer.
WORKDIR /src/StationeryStore.API
RUN dotnet publish "StationeryStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Run
# Use the lightweight ASP.NET runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
USER app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StationeryStore.API.dll"]