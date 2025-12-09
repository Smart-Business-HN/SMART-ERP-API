# Guía de Despliegue en Dokploy - SMART-ERP-API

Esta guía explica cómo desplegar la API SMART-ERP en Dokploy usando Docker.

## Requisitos Previos

- Acceso a Dokploy con permisos de administración
- Dominio configurado: `api.smartbusiness.site`
- Variables de entorno listas (ver sección de configuración)

## Configuración en Dokploy

### 1. Crear Nueva Aplicación

1. Inicia sesión en tu instancia de Dokploy
2. Ve a **Applications** → **New Application**
3. Selecciona **Docker** como tipo de aplicación
4. Configura los siguientes parámetros:

#### Configuración Básica

- **Application Name**: `smart-erp-api`
- **Domain**: `api.smartbusiness.site`
- **Port**: `8080`
- **Docker Image**: (se configurará automáticamente desde el repositorio)

### 2. Configuración del Repositorio

Si usas Git para el despliegue automático:

- **Repository URL**: URL de tu repositorio Git
- **Branch**: `main` (o la rama que uses)
- **Dockerfile Path**: `Dockerfile` (raíz del proyecto)
- **Build Context**: `.` (directorio raíz)

### 3. Variables de Entorno

Configura las siguientes variables de entorno en Dokploy. Ve a **Environment Variables** y agrega cada una:

#### Connection Strings

```
ConnectionStrings__Database=Server=tcp:smarterp.database.windows.net,1433;Initial Catalog=SmartERP;Persist Security Info=False;User ID=toor;Password=SomeThingComplicated1234;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;
```

```
ConnectionStrings__RedisServer=sb-redis.redis.cache.windows.net:6380,password=aXxJK28lOp1V0jWqQWk1zxk2tKrFnDDH1AzCaKeoDjw=,ssl=True,abortConnect=False
```

```
ConnectionStrings__AzureBlobStorage=DefaultEndpointsProtocol=https;AccountName=smarterpstorage;AccountKey=CoeNBEPVlSGjQx2wu7WoQ606DD2o70B1qFOQ6d45KUcavhso9hk8RlFS/WMzvTJ4oswRRF9Hs1Pe+AStj1OGYQ==;EndpointSuffix=core.windows.net
```

#### JWT Settings

```
JWTSettings__Key=ThebirdofHermezismynameandeatmywingstomakemetame.PleaseBenice.
```

```
JWTSettings__Issuer=www.smartbusiness.site
```

```
JWTSettings__Audience=https://www.smartbusiness.site/
```

```
JWTSettings__DurationInMinutes=7200
```

#### Sentry Configuration

```
Sentry__Dsn=https://e4e5b0906fb2111fa3c412b84577c981@o4505616174809088.ingest.sentry.io/4505616177037312
```

```
Sentry__MaxRequestBodySize=Always
```

```
Sentry__SendDefaultPii=true
```

```
Sentry__MinimumBreadcrumbLevel=Debug
```

```
Sentry__MinimumEventLevel=Error
```

```
Sentry__AttachStackTrace=true
```

```
Sentry__Debug=false
```

```
Sentry__DiagnosticsLevel=Error
```

#### Mail Settings

```
MailSettings__Mail=no-reply@smartbusiness.site
```

```
MailSettings__DisplayName=Smart Business
```

```
MailSettings__Password=Anotherdayinthemon26@
```

```
MailSettings__Host=smtp.hostinger.com
```

```
MailSettings__Port=587
```

#### Azure SignalR

```
Azure__SignalR__Enabled=true
```

```
AzureSignalRSTIUF__PrimaryConnectionString=Endpoint=https://smarterpapi.service.signalr.net;AccessKey=mzRLU703WSWxUJa8yW+1jrwJDfYMFqYTTVMNJ3PNxQs=;Version=1.0;
```

#### Access Key

```
AccessKey=8y/B?E(H+MbQeThWmZq4t7w!z$C&F)J@NcRfUjXn2r5u8x/A?D*G-KaPdSgVkYp3
```

#### Environment

```
ASPNETCORE_ENVIRONMENT=Production
```

```
ASPNETCORE_URLS=http://+:8080
```

### 4. Configuración de Red y Puertos

- **Container Port**: `8080`
- **Protocol**: `HTTP`
- **Health Check Path**: `/health` (opcional, si tienes un endpoint de health check)

### 5. Configuración de SSL/HTTPS

Dokploy maneja automáticamente el SSL a través de su proxy reverso (Nginx). Asegúrate de:

1. Habilitar **HTTPS** en la configuración del dominio
2. Configurar el certificado SSL (Let's Encrypt se puede configurar automáticamente)
3. Verificar que el dominio `api.smartbusiness.site` apunte correctamente

### 6. Configuración de Proxy Reverso

La aplicación ya está configurada para trabajar con proxy reverso (ver `Program.cs`):

- `ForwardedHeaders` está configurado para aceptar headers de Nginx/Dokploy
- La aplicación escucha en `http://+:8080` internamente
- Dokploy/Nginx manejará el HTTPS externamente

## Construcción y Despliegue

### Opción 1: Despliegue Automático desde Git

1. Conecta tu repositorio Git en Dokploy
2. Configura el webhook para despliegue automático
3. Cada push a `main` desplegará automáticamente

### Opción 2: Despliegue Manual

1. Construye la imagen localmente:
```bash
docker build -t smart-erp-api:latest .
```

2. Sube la imagen a un registro Docker (Docker Hub, GitHub Container Registry, etc.)

3. En Dokploy, configura la imagen Docker directamente

## Verificación Post-Despliegue

### 1. Verificar que la API está funcionando

```bash
curl https://api.smartbusiness.site/api/v1/health
```

O visita en el navegador:
```
https://api.smartbusiness.site/swagger
```

### 2. Verificar Logs

En Dokploy, ve a **Logs** de la aplicación para verificar:
- Que la aplicación inició correctamente
- Que se conectó a la base de datos
- Que se conectó a Redis
- Cualquier error de inicio

### 3. Verificar Conexiones

- **Base de Datos**: La aplicación debe conectarse a Azure SQL
- **Redis**: Verificar conexión al cache de Redis
- **Azure Blob Storage**: Verificar acceso al storage
- **SignalR**: Verificar conexión a Azure SignalR

## Troubleshooting

### Problema: La aplicación no inicia

**Solución**:
1. Revisa los logs en Dokploy
2. Verifica que todas las variables de entorno estén configuradas
3. Verifica que el puerto 8080 esté correctamente mapeado

### Problema: Error de conexión a la base de datos

**Solución**:
1. Verifica que la cadena de conexión esté correcta
2. Verifica que el firewall de Azure SQL permita conexiones desde la IP de Dokploy
3. Verifica que `Encrypt=True` y `TrustServerCertificate=False` estén configurados

### Problema: Error 502 Bad Gateway

**Solución**:
1. Verifica que el contenedor esté corriendo
2. Verifica que el puerto interno (8080) coincida con la configuración de Dokploy
3. Revisa los logs del contenedor

### Problema: CORS errors

**Solución**:
1. Verifica que `https://api.smartbusiness.site` esté en la lista de orígenes permitidos en `Program.cs`
2. Verifica la configuración de CORS en la aplicación

## Configuración de Health Check (Opcional)

Si quieres agregar un endpoint de health check, puedes crear un controlador simple:

```csharp
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
```

Luego actualiza el `HEALTHCHECK` en el Dockerfile si es necesario.

## Recursos Adicionales

- [Documentación de Dokploy](https://dokploy.com/docs)
- [Documentación de Docker](https://docs.docker.com/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)

## Notas Importantes

1. **Seguridad**: Nunca commits las variables de entorno sensibles al repositorio
2. **Performance**: El contenedor está optimizado con multi-stage build para reducir el tamaño
3. **Escalabilidad**: Puedes escalar horizontalmente agregando más réplicas en Dokploy
4. **Backups**: Asegúrate de tener backups regulares de la base de datos
5. **Monitoring**: Configura alertas en Sentry para errores críticos

## Actualización de la Aplicación

Para actualizar la aplicación:

1. Haz push de los cambios al repositorio (si usas despliegue automático)
2. O reconstruye y redespliega manualmente en Dokploy
3. Dokploy manejará el despliegue sin downtime si está configurado correctamente

---

**Última actualización**: 2024
**Versión de .NET**: 9.0
**Puerto interno**: 8080

