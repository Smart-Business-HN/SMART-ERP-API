# Guía de Despliegue en Dokploy

Este documento describe cómo desplegar SMART-ERP-API en Dokploy.

## Requisitos Previos

- Repositorio conectado a GitHub
- Dokploy configurado y conectado con tu cuenta de GitHub
- Base de datos SQL Server accesible
- Servidor Redis accesible (para caché)
- Cuenta de Azure Blob Storage (opcional, si usas almacenamiento)

## Configuración en Dokploy

### 1. Crear Nueva Aplicación

1. En Dokploy, ve a "Applications" y haz clic en "New Application"
2. Selecciona tu repositorio de GitHub
3. Elige la rama principal (generalmente `main` o `master`)
4. Selecciona el tipo: **Docker**

### 2. Configurar el Dockerfile

Dokploy detectará automáticamente el `Dockerfile` en la raíz del proyecto.

### 3. Variables de Entorno

Configura las siguientes variables de entorno en la sección "Environment Variables" de Dokploy:

#### Connection Strings

```
ConnectionStrings__Database=Server=tcp:tu-servidor.database.windows.net,1433;Initial Catalog=SmartERP;Persist Security Info=False;User ID=tu_usuario;Password=tu_contraseña;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;
ConnectionStrings__RedisServer=tu-redis.redis.cache.windows.net:6380,password=tu_password,ssl=True,abortConnect=False
ConnectionStrings__AzureBlobStorage=DefaultEndpointsProtocol=https;AccountName=tu_cuenta;AccountKey=tu_key;EndpointSuffix=core.windows.net
```

#### JWT Settings

```
JWTSettings__Key=Tu clave secreta JWT muy segura
JWTSettings__Issuer=https://tu-dominio.com
JWTSettings__Audience=https://tu-dominio.com/
JWTSettings__DurationInMinutes=30
```

#### Sentry (Monitoreo de Errores)

```
Sentry__Dsn=https://tu-dsn-de-sentry@sentry.io/tu-proyecto-id
Sentry__MaxRequestBodySize=Always
Sentry__SendDefaultPii=true
Sentry__MinimumBreadcrumbLevel=Debug
Sentry__MinimumEventLevel=Error
Sentry__AttachStackTrace=true
Sentry__Debug=false
Sentry__DiagnosticsLevel=Error
```

#### Mail Settings

```
MailSettings__Mail=no-reply@tu-dominio.com
MailSettings__DisplayName=Smart Business
MailSettings__Password=tu_contraseña_email
MailSettings__Host=smtp.tu-servidor.com
MailSettings__Port=587
```

#### Azure SignalR (si está habilitado)

```
Azure__SignalR__Enabled=true
AzureSignalRSTIUF__PrimaryConnectionString=Endpoint=https://tu-signalr.service.signalr.net;AccessKey=tu_key;Version=1.0;
```

#### Otros

```
AccessKey=Tu clave de acceso de aplicación
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
```

**Nota:** En Dokploy, las variables de entorno anidan usando doble guion bajo (`__`) en lugar de dos puntos (`:`).

### 4. Configuración de Puertos

- **Puerto Interno del Contenedor:** 8080
- **Puerto Externo:** Dokploy lo configurará automáticamente o puedes especificarlo

### 5. Health Check

El Dockerfile incluye un healthcheck que verifica el endpoint de Swagger. Dokploy puede usar esto para monitorear la salud de la aplicación.

### 6. Volúmenes (si es necesario)

Si necesitas almacenar archivos persistentes localmente (aunque se recomienda usar Azure Blob Storage), puedes configurar volúmenes en Dokploy.

### 7. Recursos

Asegúrate de asignar suficientes recursos:
- **RAM:** Mínimo 512MB, recomendado 1GB o más
- **CPU:** Mínimo 0.5 cores, recomendado 1 core o más

### 8. Dominio y SSL

1. Configura tu dominio en Dokploy
2. Dokploy puede configurar automáticamente SSL con Let's Encrypt

## Configuración de CORS

Asegúrate de que los orígenes CORS en `Program.cs` incluyan tu dominio de Dokploy:

```csharp
policy.WithOrigins("https://tu-dominio-en-dokploy.com")
```

## Primer Despliegue

1. Haz clic en "Deploy" en Dokploy
2. Monitorea los logs para ver el progreso del build
3. Una vez completado, verifica que la aplicación responde en `/swagger`

## Actualizaciones Futuras

Dokploy puede configurarse para hacer despliegues automáticos cuando:
- Haces push a la rama principal
- Creas un tag de release

Configura esto en la sección "Auto Deploy" de tu aplicación en Dokploy.

## Troubleshooting

### La aplicación no inicia

- Verifica los logs en Dokploy
- Asegúrate de que todas las variables de entorno están configuradas
- Verifica que la base de datos es accesible desde el servidor de Dokploy

### Error de conexión a la base de datos

- Verifica que el firewall de SQL Server permite conexiones desde la IP del servidor de Dokploy
- Verifica las credenciales de conexión
- Asegúrate de que `Encrypt` y `TrustServerCertificate` están configurados correctamente

### Error de conexión a Redis

- Verifica que Redis es accesible desde el servidor de Dokploy
- Verifica la cadena de conexión
- Asegúrate de que el firewall permite conexiones al puerto de Redis

### Swagger no está disponible

- En producción, Swagger solo está disponible si `ASPNETCORE_ENVIRONMENT` no es `Production`
- Puedes cambiar esto modificando `Program.cs` o ajustando la variable de entorno

## Notas Importantes

- **No commits información sensible:** Las contraseñas y keys deben estar solo en las variables de entorno de Dokploy
- **Backups:** Asegúrate de tener backups regulares de tu base de datos
- **Monitoreo:** Configura alertas en Sentry para estar al tanto de errores
- **Logs:** Los logs están disponibles en la interfaz de Dokploy

## Comandos Útiles

Para ver logs en tiempo real:
```bash
# En Dokploy, ve a la sección "Logs" de tu aplicación
```

Para reiniciar la aplicación:
```bash
# En Dokploy, haz clic en "Restart" en la sección de tu aplicación
```

