# F0-T2 · Matriz de dispositivos y emuladores

**Tarea:** F0-T2
**Objetivo:** cubrir los extremos de tamaño y las dos plataformas con el mínimo de dispositivos.
**Última revisión:** 22 de octubre de 2026.

## Matriz mínima

| Plataforma | Dispositivo | Versión | Por qué | Responsable | Cómo se arranca |
|---|---|---|---|---|---|
| Android | Pixel 7 (físico o emulador) | API 35 (Android 15) | Edge-to-edge, barra de gestos, recorte del icono | Bryan | `emulator -avd Pixel_7_API_35` o `adb devices` |
| Android | Emulador pequeño 360×640 | API 30 | Peor caso de ancho y de tipografía | Bryan | `emulator -avd Small_360x640_API_30` |
| Android | Samsung Galaxy A32 (disponible) | Android 13 / API 33 | Dispositivo físico real, densidad 420–480 | Bryan | `adb devices` → `RF8R21L4QND` |
| iOS | iPhone 13/14 | iOS 17 | Notch + home indicator | Bryan (requiere macOS) | Xcode → Open Developer Tool → Simulator |
| iOS | iPhone SE (3.ª gen.) | iOS 17 | Pantalla pequeña, sin notch | Bryan (requiere macOS) | Xcode → Simulator → iPhone SE (3rd generation) |
| iPad | iPad (10.ª gen.) | iPadOS 17 | Ancho grande y multitarea | Bryan (requiere macOS) | Xcode → Simulator → iPad (10th generation) |
| Escritorio | Windows 11 (solo admin) | — | `net9.0-windows` | Bryan | `dotnet build -f net9.0-windows10.0.19041.0` en Windows |

## Estado del entorno de trabajo (22/10/2026)

| Elemento | Estado | Detalle |
|---|---|---|
| Sistema operativo | Linux | Solo permite compilar Android. iOS/Mac Catalyst exigen macOS con Xcode. |
| SDK .NET | 9.0.319 en `/home/bryan/.dotnet9` | El `dotnet` por defecto es 10.0.112 y no tiene el workload de MAUI. Usar `export PATH=/home/bryan/.dotnet9:$PATH`. |
| Workload MAUI | `maui-android` | No incluye iOS (no existe para Linux). |
| Dispositivo Android | Samsung SM-A325M | Android 13 (API 33), 1080×2400, densidad 420 con override 480. Conectado por ADB como `RF8R21L4QND`. |
| Bloqueo del dispositivo | Bloqueado con PIN | No se pudieron tomar capturas automatizadas sin desbloquear el equipo. Ver F0-T3. |
| Emuladores | No verificados | Si no existen, crear uno con Android Studio o `avdmanager`. |
| macOS / Xcode | No disponible | Los pases de iOS quedan pendientes de un equipo Mac. |

## Comandos útiles

```bash
# Dispositivos conectados
adb devices -l

# Datos del dispositivo
adb shell getprop ro.product.model
adb shell getprop ro.build.version.release
adb shell getprop ro.build.version.sdk
adb shell wm size
adb shell wm density

# Emuladores disponibles / arranque
emulator -list-avds
emulator -avd <nombre-del-avd>

# Simuladores iOS (solo macOS)
xcrun simctl list devices available
open -a Simulator
```

## Rotación y tipografía

- La app no declara bloqueo de orientación: **rotar forma parte de la prueba** en móvil y tablet.
- Repetir el recorrido con el tamaño de fuente del sistema al máximo:
  - Android: Ajustes → Pantalla → Tamaño de fuente.
  - iOS: Ajustes → Accesibilidad → Pantalla y tamaño de texto.

## Criterio de aceptación

- [x] La matriz cubre Android 15 y un iPhone con notch (filas definidas).
- [x] Cada fila tiene responsable y comando de arranque.
- [ ] Los dispositivos iOS/iPadOS y el emulador API 35 están disponibles en el equipo de pruebas.
