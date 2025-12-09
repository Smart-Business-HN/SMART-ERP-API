# Solución al Error: "Address already in use" (Puerto 8080)

## Problema

Recibes este error en los logs:

```
System.IO.IOException: Failed to bind to address http://[::]:8080: address already in use.
System.Net.Sockets.SocketException (98): Address already in use
```

Y también ves este warning:

```
Overriding address(es) 'http://+:8080'. Binding to endpoints defined via IConfiguration and/or UseKestrel() instead.
```

## Causa

La aplicación está intentando escuchar en el puerto 8080 **dos veces**:
1. Una vez a través de `ASPNETCORE_URLS=http://+:8080`
2. Otra vez a través de la configuración manual de Kestrel con `options.ListenAnyIP(8080, ...)`

Esto causa un conflicto porque el puerto no puede ser usado por dos procesos/listeners al mismo tiempo.

## Solución

Se ha corregido el código para que:
- **En producción:** Solo use `ASPNETCORE_URLS` para configurar el puerto (sin configuración manual de Kestrel)
- **En desarrollo:** Use la configuración de Kestrel del `appsettings.json` si existe

## Cambios Realizados

✅ **Eliminada la configuración manual de Kestrel en producción**
✅ **Dejado que `ASPNETCORE_URLS` maneje todo en producción**
✅ **Mantenida la configuración de Kestrel solo para desarrollo**

## Verificación

Después del despliegue, deberías ver en los logs:

```
Now listening on: http://[::]:8080
Application started. Press Ctrl+C to shut down.
```

**NO deberías ver:**
- ❌ "Address already in use"
- ❌ "Overriding address(es) 'http://+:8080'"

## Si el Problema Persiste

### 1. Verificar que No Haya Múltiples Contenedores

En Dokploy:
- Verifica que solo haya **un contenedor** ejecutándose
- Si hay múltiples, detén los demás

### 2. Verificar Variables de Entorno

Asegúrate de que solo tengas:
```
ASPNETCORE_URLS=http://+:8080
```

**NO agregues:**
- Configuración de Kestrel en variables de entorno
- Múltiples variables que configuren el puerto

### 3. Reiniciar la Aplicación

1. **Detén la aplicación** en Dokploy
2. **Espera 10-15 segundos**
3. **Inicia la aplicación** nuevamente
4. **Espera a que el contenedor se inicie completamente**

### 4. Verificar el Código Esté Actualizado

Asegúrate de que el código desplegado incluya los cambios recientes:
- La configuración de Kestrel solo se aplica en desarrollo
- En producción, solo se usa `ASPNETCORE_URLS`

## Arquitectura Correcta

```
ASPNETCORE_URLS=http://+:8080  → Kestrel escucha en HTTP puerto 8080
                                → Sin configuración manual adicional
                                → Sin conflictos
```

## Notas

- El warning sobre HTTP/2 sin TLS es normal y no afecta la funcionalidad
- El warning sobre DataProtection keys es normal en contenedores
- El warning sobre wwwroot no encontrado es normal si no usas archivos estáticos

¡Con este cambio, el error "Address already in use" debería resolverse!

