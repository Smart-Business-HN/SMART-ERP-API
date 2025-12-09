# Solución al Error: "Unable to configure HTTPS endpoint"

## Problema

Recibes este error en los logs:

```
System.InvalidOperationException: Unable to configure HTTPS endpoint. No server certificate was specified, and the default developer certificate could not be found or is out of date.
```

Y también ves este warning:

```
Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'https://+:8080'.
```

## Causa

La aplicación está intentando escuchar en **HTTPS** (`https://+:8080`) pero no hay certificado SSL configurado en el contenedor. En Docker detrás de un proxy reverso (Dokploy/Nginx), la aplicación debe escuchar solo en **HTTP**, y el proxy maneja el HTTPS.

## Solución Inmediata

### 1. Verificar Variable de Entorno en Dokploy

1. **Ve a tu aplicación en Dokploy**
2. **Ve a "Settings" → "Environment Variables"**
3. **Busca la variable `ASPNETCORE_URLS`**
4. **Verifica que esté configurada como:**
   ```
   ASPNETCORE_URLS=http://+:8080
   ```
   **NO debe ser:**
   ```
   ASPNETCORE_URLS=https://+:8080  ❌ INCORRECTO
   ```

### 2. Si la Variable Está Incorrecta

1. **Edita la variable `ASPNETCORE_URLS`**
2. **Cámbiala a:** `http://+:8080` (sin la 's' de https)
3. **Guarda los cambios**
4. **Reinicia la aplicación** en Dokploy

### 3. Si la Variable No Existe

1. **Agrega una nueva variable de entorno:**
   - **Nombre:** `ASPNETCORE_URLS`
   - **Valor:** `http://+:8080`
2. **Guarda los cambios**
3. **Reinicia la aplicación**

## Cambios Realizados en el Código

Se han realizado los siguientes cambios para prevenir este problema:

✅ **Configuración de Kestrel forzada a HTTP en producción**
✅ **Dockerfile actualizado** con variables de entorno correctas
✅ **Documentación actualizada** con instrucciones claras

## Verificación

Después de corregir la variable de entorno:

1. **Espera a que la aplicación se reinicie** (30-60 segundos)
2. **Verifica los logs** - deberías ver:
   ```
   Now listening on: http://[::]:8080
   ```
   **NO deberías ver:**
   ```
   Now listening on: https://[::]:8080  ❌
   ```

3. **Prueba el acceso:**
   ```bash
   curl -I https://api.smartbusiness.site
   ```

## Por Qué HTTP y No HTTPS

En una arquitectura con proxy reverso:

```
Internet (HTTPS) → Dokploy/Nginx (HTTPS) → Contenedor Docker (HTTP) → Aplicación .NET
```

- **Dokploy/Nginx** termina la conexión HTTPS y se comunica con el contenedor en HTTP
- **La aplicación .NET** solo necesita escuchar en HTTP
- **No se necesita certificado SSL** en el contenedor

## Variables de Entorno Correctas

```
✅ ASPNETCORE_URLS=http://+:8080
✅ ASPNETCORE_ENVIRONMENT=Production
```

## Variables de Entorno Incorrectas

```
❌ ASPNETCORE_URLS=https://+:8080
❌ ASPNETCORE_URLS=http://localhost:8080
❌ ASPNETCORE_URLS=https://localhost:8080
```

## Si el Problema Persiste

1. **Verifica que no haya otras variables** sobrescribiendo `ASPNETCORE_URLS`
2. **Revisa los logs completos** para ver qué URL está intentando usar
3. **Asegúrate de que el código esté actualizado** con los cambios recientes
4. **Reconstruye la imagen** si es necesario

## Notas Adicionales

- El Dockerfile ya tiene `ENV ASPNETCORE_URLS=http://+:8080` configurado
- Si tienes una variable de entorno en Dokploy, **sobrescribirá** la del Dockerfile
- Por eso es importante verificar que la variable en Dokploy sea correcta

¡Con esta corrección, el error de HTTPS debería resolverse!

