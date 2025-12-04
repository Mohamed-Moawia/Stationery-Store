# STAGE 1: Build
# We use the SDK image (heavy, contains compilers) only for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy ONLY the csproj file first
# This isolates the "Restore" step. 
# If you change code but not dependencies, Docker skips this step (Cache Hit!)
COPY ["StationeryStore/StationeryStore.csproj", "StationeryStore/"]
RUN dotnet restore "StationeryStore/StationeryStore.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/StationeryStore"

# Build and Publish (Release mode is crucial for performance)
RUN dotnet build "StationeryStore.csproj" -c Release -o /app/build
RUN dotnet publish "StationeryStore.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Run
# We use the ASP.NET runtime image (lightweight, no compilers)
# This reduces attack surface (hackers can't compile malware inside the container)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Create a non-root user for security (Best Practice)
# By default, Docker runs as root. If compromised, the attacker has root inside the container.
USER app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StationeryStore.dll"]