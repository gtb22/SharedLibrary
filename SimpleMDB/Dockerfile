# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/SimpleMDB.Core/SimpleMDB.Core.csproj", "src/SimpleMDB.Core/"]
COPY ["src/SimpleMDB.Api/SimpleMDB.Api.csproj", "src/SimpleMDB.Api/"]
RUN dotnet restore "src/SimpleMDB.Api/SimpleMDB.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/SimpleMDB.Api"
RUN dotnet build "SimpleMDB.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SimpleMDB.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY WWWRoot ./WWWRoot
ENTRYPOINT ["dotnet", "SimpleMDB.Api.dll"]
