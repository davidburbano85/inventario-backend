# Imagen base para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app

EXPOSE 8080


# Imagen de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src


# Copiar proyecto y restaurar dependencias
COPY *.csproj ./

RUN dotnet restore


# Copiar código completo
COPY . ./


# Publicar aplicación
RUN dotnet publish -c Release -o /app/publish


# Imagen final
FROM base AS final

WORKDIR /app


# Copiar publicación
COPY --from=build /app/publish .


ENTRYPOINT ["dotnet", "inventarioWebAI.dll"]