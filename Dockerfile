# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY ["SMART.ERP.sln", "./"]
COPY ["SMART.ERP.API/SMART.ERP.API.csproj", "SMART.ERP.API/"]
COPY ["SMART.ERP.Application/SMART.ERP.Application.csproj", "SMART.ERP.Application/"]
COPY ["SMART.ERP.Domain/SMART.ERP.Domain.csproj", "SMART.ERP.Domain/"]
COPY ["SMART.ERP.Infrastructure/SMART.ERP.Infrastructure.csproj", "SMART.ERP.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "SMART.ERP.sln"

# Copiar todo el código fuente
COPY . .

# Build y publicar la aplicación
WORKDIR "/src/SMART.ERP.API"
RUN dotnet build "SMART.ERP.API.csproj" -c Release -o /app/build
RUN dotnet publish "SMART.ERP.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Crear usuario no-root para seguridad
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copiar archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Cambiar ownership de los archivos
RUN chown -R appuser:appuser /app

# Cambiar al usuario no-root
USER appuser

# Exponer el puerto 8080 (configurado para Dokploy)
EXPOSE 8080

# Variables de entorno por defecto
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Health check opcional (comentado - descomentar si tienes endpoint /health)
# HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
#   CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Punto de entrada
ENTRYPOINT ["dotnet", "SMART.ERP.API.dll"]

