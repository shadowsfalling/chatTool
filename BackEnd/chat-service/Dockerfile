# Verwende das .NET 8 SDK-Image zum Bauen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Kopiere csproj und restore Abhängigkeiten
COPY *.csproj ./
RUN dotnet restore

# Kopiere den Rest der Anwendung und baue sie
COPY . ./
RUN dotnet publish -c Release -o out

# Verwende das .NET 8 Runtime-Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Kopiere das erstellte Build-Output
COPY --from=build-env /app/out .

# Setze den Startbefehl für die Anwendung
ENTRYPOINT ["dotnet", "chat-service.dll"]