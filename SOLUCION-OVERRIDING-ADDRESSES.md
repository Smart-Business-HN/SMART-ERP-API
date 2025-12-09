# Solución al Warning: "Overriding address(es) 'http://+:8080'"

## Problema

Ves este warning en los logs:

```
Overriding address(es) 'http://+:8080'. Binding to endpoints defined via IConfiguration and/or UseKestrel() instead.
```

Esto significa que hay alguna configuración que está **sobrescribiendo** `ASPNETCORE_URLS=http://+:8080` y estableciendo un puerto diferente, causando que Dokploy no pueda conectarse (502 Bad Gateway).

## Causa

.NET Core está detectando configuración de Kestrel desde alguna de estas fuentes:
1. **appsettings.json** o **appsettings.Production.json** con sección `Kestrel`
2. **Variables de entorno** con prefijo `Kestrel__`
3. **Configuración manual** en `Program.cs` con `Configure<KestrelServerOptions>`

Cuando esto ocurre, Kestrel **ignora** `ASPNETCORE_URLS` y usa la configuración encontrada, que probablemente apunta a un puerto diferente (como 5000, 7000, etc.).

## Solución Aplicada

Se ha actualizado `Program.cs` para:
- ✅ **En producción:** Limpiar explícitamente cualquier configuración de endpoints de Kestrel
- ✅ **Forzar que solo use `ASPNETCORE_URLS`** en producción
- ✅ **En desarrollo:** Permitir configuración de Kestrel desde appsettings

## Verificación en Dokploy

### 1. Verificar Variables de Entorno

En Dokploy → Settings → Environment Variables, **NO debe haber**:

❌ `Kestrel__Endpoints__Http__Url`
❌ `Kestrel__Endpoints__Https__Url`
❌ Cualquier variable que empiece con `Kestrel__`

✅ **Solo debe haber:**
```
ASPNETCORE_URLS=http://+:8080
ASPNETCORE_ENVIRONMENT=Production
```

### 2. Verificar Archivos de Configuración

Asegúrate de que **NO exista** `appsettings.Production.json` con configuración de Kestrel.

Si existe, debe estar vacío o sin la sección `Kestrel`.

### 3. Verificar Logs Después del Despliegue

Después del despliegue, los logs deberían mostrar:

```
Now listening on: http://[::]:8080
Application started. Press Ctrl+C to shut down.
```

**NO deberías ver:**
- ❌ "Overriding address(es) 'http://+:8080'"
- ❌ "Now listening on: http://[::]:5000" (u otro puerto)
- ❌ "Binding to endpoints defined via IConfiguration"

## Si el Problema Persiste

### Opción 1: Verificar en el Contenedor

En Dokploy → App → Terminal, ejecuta:

```bash
# Ver en qué puerto está escuchando realmente
netstat -tlnp | grep LISTEN

# O
ss -tlnp | grep LISTEN
```

Si ves que está escuchando en un puerto diferente a 8080, hay configuración que está sobrescribiendo.

### Opción 2: Verificar Variables de Entorno en el Contenedor

En Dokploy → App → Terminal:

```bash
# Ver todas las variables de entorno relacionadas con Kestrel
env | grep -i kestrel

# Ver ASPNETCORE_URLS
echo $ASPNETCORE_URLS
```

### Opción 3: Verificar Archivos de Configuración

Si tienes acceso al contenedor:

```bash
# Ver appsettings.json
cat /app/appsettings.json | grep -i kestrel

# Ver si existe appsettings.Production.json
ls -la /app/appsettings*.json
```

## Configuración Correcta

### En Dokploy (Variables de Entorno)

```
✅ ASPNETCORE_URLS=http://+:8080
✅ ASPNETCORE_ENVIRONMENT=Production
```

### En appsettings.json

```json
{
  // NO debe haber sección "Kestrel" aquí
  "ConnectionStrings": { ... },
  // ... resto de configuración
}
```

### En Program.cs

```csharp
// En producción, limpiar configuración de Kestrel
if (builder.Environment.IsProduction())
{
    builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.Endpoints.Clear(); // Forzar uso de ASPNETCORE_URLS
    });
}
```

## Orden de Precedencia de Configuración

.NET Core carga la configuración en este orden (mayor a menor prioridad):

1. **Variables de entorno** (más alta prioridad)
2. **appsettings.{Environment}.json**
3. **appsettings.json**
4. **Código en Program.cs**

Por eso es importante:
- ✅ Usar `ASPNETCORE_URLS` como variable de entorno
- ✅ NO tener configuración de Kestrel en appsettings
- ✅ Limpiar configuración de Kestrel en código para producción

## Cambios Realizados

✅ **Program.cs actualizado** para limpiar endpoints de Kestrel en producción
✅ **Documentación actualizada** con instrucciones claras
✅ **Verificación de variables de entorno** en Dokploy

¡Con estos cambios, el warning debería desaparecer y la aplicación debería escuchar correctamente en el puerto 8080!

