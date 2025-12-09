# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY ["SMART.ERP.API/SMART.ERP.API.csproj", "SMART.ERP.API/"]
COPY ["SMART.ERP.Application/SMART.ERP.Application.csproj", "SMART.ERP.Application/"]
COPY ["SMART.ERP.Domain/SMART.ERP.Domain.csproj", "SMART.ERP.Domain/"]
COPY ["SMART.ERP.Infrastructure/SMART.ERP.Infrastructure.csproj", "SMART.ERP.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "SMART.ERP.API/SMART.ERP.API.csproj"

# Copiar todo el código fuente
COPY . .

# Construir la aplicación
WORKDIR "/src/SMART.ERP.API"
RUN dotnet build "SMART.ERP.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "SMART.ERP.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instalar curl para healthcheck (imagen base no lo incluye)
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

# Crear usuario no root para seguridad
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Copiar assets estáticos desde la etapa de build (necesarios para MailService)
COPY --from=build /src/SMART.ERP.API/Assets ./Assets

# Cambiar propiedad de todos los archivos al usuario no root
RUN chown -R appuser:appuser /app

# Cambiar al usuario no root
USER appuser

# Exponer puerto (Dokploy puede mapear este puerto)
EXPOSE 8080

# Variables de entorno por defecto
# IMPORTANTE: Usar HTTP (no HTTPS) porque Dokploy/Nginx maneja HTTPS
# Escuchar en todas las interfaces (0.0.0.0) para IPv4 e IPv6
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Healthcheck - verifica que la aplicación responda
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/ || exit 1

# Punto de entrada
ENTRYPOINT ["dotnet", "SMART.ERP.API.dll"]

