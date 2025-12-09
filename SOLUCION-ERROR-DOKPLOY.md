# Solución al Error: "failed to read dockerfile"

## Problema

Estás recibiendo este error en Dokploy:
```
ERROR: failed to build: failed to solve: failed to read dockerfile: open code: no such file or directory
```

## Causa

El campo **"Docker File"** en la configuración de Dokploy está mal configurado. Probablemente tiene solo `/` cuando debería tener el nombre del archivo.

## Solución Paso a Paso

1. **Ve a tu aplicación en Dokploy** y haz clic en "Settings" o "Configuration"

2. **Busca la sección "Build Type"** o "Docker Settings"

3. **Ajusta los siguientes campos:**

   ```
   Build Type: Dockerfile
   Docker File: Dockerfile          ← CAMBIAR ESTO (no usar "/")
   Docker Context Path: .           ← Debe ser un punto
   Docker Build Stage: (vacío)      ← Puede estar vacío o poner "final"
   Build Path: (vacío o "/")        ← Puede estar vacío
   ```

4. **Configuración específica recomendada:**

   - **Docker File:** `Dockerfile` (exactamente así, sin barras)
   - **Docker Context Path:** `.` (un punto, significa directorio actual)
   - **Build Path:** Déjalo vacío o usa `/` si es necesario

5. **Guarda los cambios** y vuelve a intentar el despliegue

## Verificación

Después de cambiar la configuración, cuando Dokploy intente construir la imagen, debería:

1. Encontrar el Dockerfile en la raíz del repositorio
2. Comenzar a construir las capas de la imagen
3. Progresar sin el error anterior

## Si el Problema Persiste

1. **Verifica que el Dockerfile esté en la raíz del repositorio:**
   - Debe estar en: `/SMART-ERP-API/Dockerfile`
   - No dentro de una subcarpeta

2. **Verifica que el Dockerfile esté en el repositorio de GitHub:**
   - Haz commit y push del Dockerfile si aún no lo has hecho
   - Verifica en GitHub que el archivo esté presente

3. **Prueba con una configuración alternativa:**
   - **Docker File:** `./Dockerfile`
   - **Docker Context Path:** `.`

## Configuración Completa Recomendada

```
GitHub Account: [Tu cuenta]
Repository: SMART-ERP-API
Branch: main
Build Path: (vacío)
Trigger Type: On Push

Build Type: Dockerfile
Docker File: Dockerfile
Docker Context Path: .
Docker Build Stage: (vacío)
```

¡Esto debería resolver el error!



