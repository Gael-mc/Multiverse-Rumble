# Dockerfile para desplegar Multiverse-Rumble (sitio MVC) en Render/cualquier host con Docker.
# Solo publica el proyecto del juego (Multiverse-Rumble); la API y los tests no se necesitan
# en este contenedor de producción.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Multiverse-Rumble/Multiverse-Rumble.csproj Multiverse-Rumble/
RUN dotnet restore Multiverse-Rumble/Multiverse-Rumble.csproj

COPY Multiverse-Rumble/ Multiverse-Rumble/
RUN dotnet publish Multiverse-Rumble/Multiverse-Rumble.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "Multiverse-Rumble.dll"]
