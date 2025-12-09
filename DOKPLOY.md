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

### 2. Configurar el Dockerfile y Build Settings

En la sección de configuración de GitHub y Build, configura lo siguiente:

#### Configuración de GitHub:
- **Github Account:** Selecciona tu cuenta de GitHub conectada (ej: "Dokploy-smartbusiness")
- **Repository:** Selecciona "SMART-ERP-API"
- **Branch:** Selecciona "main" (o la rama que quieras usar)
- **Build Path:** Debe estar vacío o configurado como `/` (raíz del repositorio)
- **Trigger Type:** "On Push" (para despliegues automáticos)

#### Configuración de Build Type:
- **Build Type:** Selecciona **"Dockerfile"**
- **Docker File:** Debe estar configurado como `Dockerfile` (sin la barra inicial) o simplemente `./Dockerfile`. **IMPORTANTE:** No uses solo `/` porque causará el error "failed to read dockerfile"
- **Docker Context Path:** Debe estar configurado como `.` (punto, que significa el directorio actual)
- **Docker Build Stage:** Déjalo vacío (o pon `final` si quieres especificar la etapa final del multi-stage build)

**⚠️ Solución al Error:** Si ves el error `failed to read dockerfile: open code: no such file or directory`, asegúrate de que el campo "Docker File" contenga exactamente `Dockerfile` y no `/`.

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

**⚠️ IMPORTANTE sobre ASPNETCORE_URLS:**
- ✅ **CORRECTO:** `ASPNETCORE_URLS=http://+:8080` (HTTP, sin la 's')
- ❌ **INCORRECTO:** `ASPNETCORE_URLS=https://+:8080` (causará error de certificado SSL)
- El proxy reverso (Dokploy/Nginx) maneja HTTPS, la aplicación .NET solo necesita escuchar en HTTP

**Nota:** En Dokploy, las variables de entorno anidan usando doble guion bajo (`__`) en lugar de dos puntos (`:`).

### 4. Configuración de Puertos y Dominio

#### Configuración de Puerto:
- **Puerto Interno del Contenedor:** 8080 (debe coincidir con `EXPOSE 8080` en Dockerfile)
- **En Dokploy:** Ve a la configuración de tu aplicación y asegúrate de que el puerto interno esté configurado como **8080**
- **Puerto Externo:** Dokploy lo configurará automáticamente

#### Configuración de Dominio:
1. **Agrega el dominio en Dokploy:**
   - Ve a la configuración de tu aplicación
   - Busca la sección "Domains" o "Custom Domain"
   - Agrega: `api.smartbusiness.site`

2. **Configuración DNS:**
   - Asegúrate de que los registros DNS apunten correctamente al servidor de Dokploy
   - Tipo A o CNAME apuntando a la IP de Dokploy

3. **SSL/TLS:**
   - Dokploy puede configurar automáticamente SSL con Let's Encrypt
   - Verifica que SSL esté habilitado para tu dominio

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

### Error: "failed to read dockerfile: open code: no such file or directory"

Este error ocurre cuando la configuración del Dockerfile en Dokploy es incorrecta. Solución:

1. Ve a la configuración de tu aplicación en Dokploy
2. En la sección "Build Type", verifica:
   - **Docker File:** Debe contener exactamente `Dockerfile` (sin barras iniciales)
   - **Docker Context Path:** Debe ser `.` (un punto)
   - **Build Path:** Debe estar vacío o ser `/`

Si el campo "Docker File" contiene solo `/` o está vacío, cámbialo a `Dockerfile`.

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

### Error: Bad Gateway (502)

Este error generalmente ocurre cuando el proxy reverso (Dokploy/Nginx) no puede comunicarse con el contenedor. Verifica:

1. **Puerto Interno Configurado Correctamente:**
   - En Dokploy, verifica que el puerto interno del contenedor esté configurado como **8080**
   - Debe coincidir con `EXPOSE 8080` en el Dockerfile

2. **La aplicación está escuchando correctamente:**
   - Verifica los logs de la aplicación en Dokploy
   - Asegúrate de que veas mensajes como "Now listening on: http://[::]:8080"

3. **Variables de Entorno:**
   - Verifica que `ASPNETCORE_URLS=http://+:8080` esté configurado
   - O asegúrate de que el Dockerfile tenga esta variable (ya está incluida)

4. **Healthcheck:**
   - Verifica que el contenedor esté saludable
   - El healthcheck debería pasar después de ~40 segundos

5. **Headers de Proxy:**
   - El código ya está configurado con `UseForwardedHeaders()` para manejar correctamente los headers del proxy
   - Asegúrate de que el despliegue incluya estos cambios

6. **Reinicia la aplicación:**
   - Después de hacer cambios, reinicia la aplicación en Dokploy
   - Espera a que el contenedor se inicie completamente antes de probar

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

