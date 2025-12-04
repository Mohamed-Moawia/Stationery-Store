# Dockerfile
# Multi-stage build for Egyptian deployment

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Set Egyptian locale for build
ENV LANG=ar_EG.UTF-8
ENV LC_ALL=ar_EG.UTF-8
RUN apt-get update && apt-get install -y locales && \
    locale-gen ar_EG.UTF-8 && \
    update-locale LANG=ar_EG.UTF-8

# Copy solution and restore
COPY ["StationeryStore.sln", "./"]
COPY ["StationeryStore.API/StationeryStore.API.csproj", "StationeryStore.API/"]
COPY ["StationeryStore.Application/StationeryStore.Application.csproj", "StationeryStore.Application/"]
COPY ["StationeryStore.Domain/StationeryStore.Domain.csproj", "StationeryStore.Domain/"]
COPY ["StationeryStore.Infrastructure/StationeryStore.Infrastructure.csproj", "StationeryStore.Infrastructure/"]
COPY ["StationeryStore.Tests/StationeryStore.Tests.csproj", "StationeryStore.Tests/"]

RUN dotnet restore "StationeryStore.sln"

# Copy everything else and build
COPY . .
WORKDIR "/src/StationeryStore.API"
RUN dotnet build "StationeryStore.API.csproj" -c Release -o /app/build \
    /p:IncludeEgyptianLocalization=true

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "StationeryStore.API.csproj" -c Release -o /app/publish \
    /p:UseAppHost=false \
    /p:IncludeEgyptianLocalization=true

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install Egyptian timezone data
RUN apt-get update && apt-get install -y \
    tzdata \
    locales \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Set Egyptian timezone and locale
ENV TZ=Africa/Cairo
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
ENV LANG=ar_EG.UTF-8
ENV LC_ALL=ar_EG.UTF-8
RUN locale-gen ar_EG.UTF-8 && update-locale LANG=ar_EG.UTF-8

# Create non-root user for security (Egyptian security compliance)
RUN groupadd -r egyptuser && useradd -r -g egyptuser -s /bin/false egyptuser

# Copy published app
COPY --from=publish /app/publish .

# Create directories for Egyptian logs and certificates
RUN mkdir -p /app/logs /app/certs /app/eta-certificates && \
    chown -R egyptuser:egyptuser /app && \
    chmod 755 /app

# Switch to non-root user
USER egyptuser

# Health check endpoint for Egyptian deployment
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose Egyptian standard port
EXPOSE 8080

# Set environment variables for Egypt
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV Egypt__TimeZone=Africa/Cairo
ENV Egypt__DefaultLanguage=ar
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "StationeryStore.API.dll"]