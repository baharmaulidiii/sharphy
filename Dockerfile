# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy Solution & configuration properties
COPY Directory.Build.props nuget.config* ./
COPY src/Sharphy.Core/Sharphy.Core.csproj src/Sharphy.Core/
COPY src/Sharphy.Server/Sharphy.Server.csproj src/Sharphy.Server/
COPY src/Sharphy.Cli/Sharphy.Cli.csproj src/Sharphy.Cli/

# Restore dependencies for Sharphy.Cli
RUN dotnet restore src/Sharphy.Cli/Sharphy.Cli.csproj

# Copy all source files
COPY src/ src/

# Publish Sharphy.Cli targeting net9.0
RUN dotnet publish src/Sharphy.Cli/Sharphy.Cli.csproj \
    -c Release \
    -f net9.0 \
    -o /app/publish \
    --no-restore

# Stage 2: Minimal Runtime Container
FROM mcr.microsoft.com/dotnet/runtime:9.0-noble-chiseled
WORKDIR /app
COPY --from=build /app/publish .

# Pure Stdio JSON-RPC entrypoint for Smithery
ENTRYPOINT ["dotnet", "Sharphy.Cli.dll", "mcp", "start"]
