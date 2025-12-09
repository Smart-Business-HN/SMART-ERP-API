# Solución al Error: Bad Gateway (502) en Dokploy

## Problema

Al acceder a `https://api.smartbusiness.site` recibes un error **Bad Gateway (502)**.

## Causa

El error Bad Gateway generalmente ocurre cuando el proxy reverso (Dokploy/Nginx) no puede comunicarse con tu aplicación .NET en el contenedor.

## Solución Paso a Paso

### 1. Verificar Configuración de Puerto en Dokploy

1. **Ve a tu aplicación en Dokploy**
2. **Busca la sección "Settings" o "Configuration"**
3. **Verifica la configuración de puerto:**
   - Debe estar configurado el puerto interno: **8080**
   - Esto debe coincidir con `EXPOSE 8080` en tu Dockerfile

### 2. Verificar que la Aplicación Está Escuchando

1. **Ve a la sección "Logs" en Dokploy**
2. **Busca mensajes como:**
   ```
   Now listening on: http://[::]:8080
   Application started. Press Ctrl+C to shut down.
   ```
3. **Si no ves estos mensajes**, la aplicación no está iniciando correctamente

### 3. Verificar Variables de Entorno

**⚠️ CRÍTICO:** Asegúrate de que estas variables estén configuradas correctamente en Dokploy:

```
ASPNETCORE_URLS=http://+:8080
ASPNETCORE_ENVIRONMENT=Production
```

**IMPORTANTE:** 
- ❌ **NO uses** `https://+:8080` - esto causará el error de certificado SSL
- ✅ **DEBE ser** `http://+:8080` (sin la 's' de https)
- El proxy reverso (Dokploy/Nginx) maneja HTTPS, la aplicación solo necesita HTTP

Si tienes `ASPNETCORE_URLS` configurada como `https://+:8080`, cámbiala inmediatamente a `http://+:8080`.

### 4. Verificar Healthcheck

1. **Espera al menos 40-60 segundos** después del despliegue
2. **Verifica el estado del contenedor** en Dokploy
3. **El healthcheck debe pasar** (debería mostrar "healthy")

### 5. Verificar Configuración del Dominio

1. **Ve a la configuración de dominios** en Dokploy
2. **Verifica que `api.smartbusiness.site` esté agregado**
3. **Asegúrate de que SSL esté configurado** (Let's Encrypt)

### 6. Cambios Realizados en el Código

Se han realizado los siguientes cambios para resolver el problema:

✅ **Configuración mejorada de ForwardedHeaders** para proxy reverso
✅ **Agregado `api.smartbusiness.site` a CORS**
✅ **Agregado `api.smartbusiness.site` a WebSockets**
✅ **Deshabilitado HttpsRedirection** (Dokploy ya maneja HTTPS)

### 7. Pasos de Despliegue

Después de hacer estos cambios:

1. **Haz commit y push de los cambios:**
   ```bash
   git add .
   git commit -m "Fix: Configuración para Dokploy y dominio api.smartbusiness.site"
   git push
   ```

2. **En Dokploy, haz un nuevo despliegue** (automático si tienes auto-deploy o manual)

3. **Espera a que el build termine** (puede tardar varios minutos)

4. **Espera otros 40-60 segundos** para que el contenedor inicie completamente

5. **Verifica los logs** para confirmar que la aplicación está escuchando

6. **Prueba el acceso:**
   - `https://api.smartbusiness.site` (debe responder)
   - `https://api.smartbusiness.site/swagger` (si quieres ver Swagger)

## Verificación Final

### Verificar que el Contenedor Está Saludable

En los logs de Dokploy deberías ver:
- ✅ Build exitoso
- ✅ Contenedor iniciado
- ✅ "Now listening on: http://[::]:8080"
- ✅ Healthcheck pasando

### Probar el Endpoint

```bash
# Debe responder con algún contenido (aunque sea un error 404 o similar, no 502)
curl -I https://api.smartbusiness.site
```

### Verificar Swagger (si está habilitado)

Si tienes Swagger habilitado en producción:
```
https://api.smartbusiness.site/swagger
```

## Si el Problema Persiste

### 1. Verifica los Logs Detallados

- Revisa los logs completos en Dokploy
- Busca errores específicos de .NET
- Verifica errores de conexión a la base de datos

### 2. Verifica la Conectividad del Contenedor

- Asegúrate de que el contenedor pueda comunicarse con servicios externos (BD, Redis)
- Verifica que no haya problemas de firewall

### 3. Verifica la Configuración de Red en Dokploy

- Asegúrate de que el proxy reverso esté correctamente configurado
- Verifica que el dominio esté correctamente vinculado a la aplicación

### 4. Reinicia la Aplicación

En Dokploy:
- Detén la aplicación
- Iníciala nuevamente
- Espera a que el contenedor esté completamente iniciado

## Configuración Recomendada en Dokploy

```
Puerto Interno: 8080
Puerto Externo: (automático)
Dominio: api.smartbusiness.site
SSL: Habilitado (Let's Encrypt)
Healthcheck: Habilitado
```

## Variables de Entorno Esenciales

```
ASPNETCORE_URLS=http://+:8080
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Database=...
ConnectionStrings__RedisServer=...
```

¡Con estos cambios, el Bad Gateway debería resolverse!

