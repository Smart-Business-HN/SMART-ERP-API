# Configurar Redis de Dokploy

## Connection String Proporcionada

Dokploy te dio esta URL:
```
redis://default:aXxJK28lOp1V0jWqQWk1zxk2tKrFnDDH1AzCaKeoDjw=@smart-business-redis-server-ajehyo:6379
```

## Formato para .NET (StackExchange.Redis)

Para .NET, necesitas convertirla al formato de StackExchange.Redis:

```
smart-business-redis-server-ajehyo:6379,password=aXxJK28lOp1V0jWqQWk1zxk2tKrFnDDH1AzCaKeoDjw=,ssl=False,abortConnect=False
```

**Nota:** `ssl=False` porque es una conexión interna dentro de Docker (no necesitas SSL entre contenedores).

## Dónde Agregarla

### Opción 1: Variables de Entorno en Dokploy (RECOMENDADO)

1. **Ve a tu aplicación en Dokploy**
2. **Settings → Environment Variables**
3. **Agrega o edita la variable:**

   **Nombre:**
   ```
   ConnectionStrings__RedisServer
   ```

   **Valor:**
   ```
   smart-business-redis-server-ajehyo:6379,password=aXxJK28lOp1V0jWqQWk1zxk2tKrFnDDH1AzCaKeoDjw=,ssl=False,abortConnect=False
   ```

4. **Guarda los cambios**
5. **Reinicia la aplicación**

### Opción 2: Actualizar appsettings.json (No recomendado para producción)

Si quieres actualizarlo en el código (solo para desarrollo local):

```json
{
  "ConnectionStrings": {
    "RedisServer": "smart-business-redis-server-ajehyo:6379,password=aXxJK28lOp1V0jWqQWk1zxk2tKrFnDDH1AzCaKeoDjw=,ssl=False,abortConnect=False"
  }
}
```

**⚠️ IMPORTANTE:** No commitees credenciales en el código. Usa variables de entorno en producción.

## Verificación

Después de configurar:

1. **Reinicia la aplicación** en Dokploy
2. **Verifica los logs** - no deberías ver errores de conexión a Redis
3. **Prueba el cache** - las respuestas deberían estar siendo cacheadas

## Formato Completo de la Connection String

```
host:port,password=xxx,ssl=False,abortConnect=False
```

- **host:** `smart-business-redis-server-ajehyo` (nombre del servicio en Dokploy)
- **port:** `6379` (puerto estándar de Redis)
- **password:** `aXxJK28lOp1V0jWqQWk1zxk2tKrFnDDH1AzCaKeoDjw=`
- **ssl:** `False` (conexión interna, no necesita SSL)
- **abortConnect:** `False` (intentar reconectar si falla)

## Si Tienes Problemas de Conexión

1. **Verifica que el servicio Redis esté corriendo** en Dokploy
2. **Verifica que ambos servicios estén en la misma red** (Dokploy maneja esto automáticamente)
3. **Revisa los logs** de la aplicación para ver errores específicos
4. **Prueba la conexión** desde el contenedor:
   ```bash
   # En Dokploy → App → Terminal
   telnet smart-business-redis-server-ajehyo 6379
   ```

## Nota sobre el Nombre del Host

El nombre `smart-business-redis-server-ajehyo` es el nombre del servicio en Dokploy. 
Dokploy crea automáticamente un DNS interno para que los contenedores se comuniquen por nombre.

¡Con esto deberías poder conectarte a tu Redis en Dokploy!

