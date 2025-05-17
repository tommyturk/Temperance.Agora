# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source 

# --- Restore ---
# Copy solution and all project files first for layer caching.
# Paths are relative to the build context root (Temperance.CONDUCTOR).
# Destinations are relative to WORKDIR (/source).
COPY Temperance.Agora.sln .
COPY Temperance.Agora.csproj .

# Add COPY lines here if you have other referenced projects

# Restore the entire solution - this finds all project references correctly
RUN dotnet restore Temperance.Agora.sln

# --- Build ---
# Copy the rest of the source code. Source is the build context (Temperance.Agora folder)
COPY . .

# Publish the specific project (Temperance.Agora.csproj is directly in /source)
# WORKDIR is still /source
ARG BUILD_CONFIG=Release
# Specify the project file explicitly
RUN dotnet publish Temperance.Agora.csproj --no-restore -c $BUILD_CONFIG -o /app/publish

# --- Runtime Image ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
EXPOSE 8081
# The DLL name typically matches the assembly name defined in the csproj, usually the project name.
ENTRYPOINT ["dotnet", "Temperance.Agora.dll"]