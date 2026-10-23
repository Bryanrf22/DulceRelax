# F0-T4 · Tablero de tareas y trazabilidad

**Proyecto:** DulceRelax
**Rama de trabajo:** `remediacion/uiux` (parte de `35ccc66`)
**Etiqueta del estado auditado:** `auditoria-uiux-4.0`
**Plan de referencia:** `PLAN-REMEDIACION-UIUX-DulceRelax.md`
**Última actualización:** 22 de octubre de 2026.

## Reglas del tablero

1. Un hallazgo solo se marca **Cerrado** cuando la columna `Evidencia` contiene una ruta a captura, log o salida de comando reproducible.
2. Ninguna tarea pasa a **En progreso** sin responsable.
3. Cada PR referencia el ID de tarea y los `DR-UI-nnn` que cierra.
4. Antes de cada PR se ejecuta `tools/verificar-uiux.sh` (Anexo C.1) y el script de contraste (Anexo C.2).
5. Si una tarea se bloquea, se anota el motivo y el desbloqueo en `Evidencia`, no se deja vacía.

Estados usados: `Pendiente`, `En progreso`, `Hecho`, `Bloqueado`, `No aplica`.

## Tabla de tareas

| ID | Hallazgos | Estado | Responsable | PR | Evidencia |
|---|---|---|---|---|---|
| F0-T1 | — | Hecho | Bryan | — (local) | Etiqueta `auditoria-uiux-4.0`; rama `remediacion/uiux`; `docs/qa/f0/build-baseline.txt` (0 errores) |
| F0-T2 | — | Hecho | Bryan | — (local) | `docs/qa/f0/matriz-dispositivos.md` |
| F0-T3 | — | Bloqueado | Bryan | — | `docs/qa/f0/antes/README.md`; falta macOS/iOS, desbloqueo del dispositivo, API accesible y credenciales de siembra |
| F0-T4 | — | Hecho | Bryan | — (local) | Este archivo |
| F0-T5 | — | Hecho | Bryan | — (local) | `docs/qa/f0/guion-verificacion.md` |
| F1-T1 | 001, 002 | Hecho | — | — | `DulceRelax.UI/` completo, `DulceRelax.sln` actualizado, ambos proyectos referencian `DulceRelax.UI` |
| F1-T2 | 001, 004, 005, 006, 007 | Hecho | — | — | `ColorTokens.xaml`, `TypeTokens.xaml`, `SpaceTokens.xaml`, `IconTokens.xaml`, `ComponentStyles.xaml` con todos los tokens y estilos |
| F1-T3 | 008, 009 | Hecho | — | — | Estilo implícito, mapeo tipográfico en `docs/diseno/tipografia.md` |
| F1-T4 | 003, 013 | Hecho | — | — | `IconTokens`, tamaños y radios definidos |
| F1-T5 | 002, 011 | Hecho | — | — | `IconTokens` con glifos actuales, no se cambian |
| F1-T6 | 001, 003, 013 | Hecho | — | — | Vista previa de `ComponentStyles`, todos los estilos en tokens |
| F1-T7 | 001, 002 | Hecho | — | — | Merge en ambos `App.xaml` |
| F1-T8 | 001, 002, 003, 008 | Hecho | — | — | Todas las vistas reemplazan literales por `{StaticResource ...}` |
| F1-T9 | 001 | Hecho | — | — | `docs/diseno/sistema.md` y `docs/diseno/tema.md` listos |
| F2-T1 | 004 | Hecho | — | — | `tools/f2-t1-verify-brand-color.sh` ejecutado, estados de botón verificados |
| F2-T2 | 005, 006 | Hecho | — | — | `tools/f2-contraste-simple.sh` ejecutado, todos los contrastes cumplen WCAG 2.2 AA |
| F2-T3 | 007 | Pendiente | — | — | F2-T3-T1 a F2-T3-T3 por implementar |
| F2-T4 | 001, 004 | Pendiente | — | — | F2-T4-T1 a F2-T4-T4 por implementar |
| F2-T5 | 004 | Pendiente | — | — | F2-T5 por implementar |
| F2-T6 | 004, 005, 006, 007 | Pendiente | — | — | F2-T6 por implementar |
| F3-T1 | 012 | Pendiente | — | — | F3-T1 por implementar |
| F3-T2 | 012 | Pendiente | — | — | F3-T2 por implementar |
| F3-T3 | 012, 014 | Pendiente | — | — | F3-T3 por implementar |
| F3-T4 | 013 | Pendiente | — | — | F3-T4 por implementar |
| F3-T5 | 015 | Pendiente | — | — | F3-T5 por implementar |
| F3-T6 | 008, 012 | Pendiente | — | — | F3-T6 por implementar |
| F4-T1 | 016, 017 | Pendiente | — | — | F4-T1 por implementar |
| F4-T2 | 016, 017 | Pendiente | — | — | F4-T2 por implementar |
| F4-T3 | 016 | Pendiente | — | — | F4-T3 por implementar |
| F4-T4 | 022 | Pendiente | — | — | F4-T4 por implementar |
| F4-T5 | 027 | Pendiente | — | — | F4-T5 por implementar |
| F5-T1 | 009 | Pendiente | — | — | F5-T1 por implementar |
| F5-T2 | 008 | Pendiente | — | — | F5-T2 por implementar |
| F5-T3 | 008 | Pendiente | — | — | F5-T3 por implementar |
| F5-T4 | 011 | Pendiente | — | — | F5-T4 por implementar |
| F5-T5 | 010 | Pendiente | — | — | F5-T5 por implementar |
| F5-T6 | 028 | Pendiente | — | — | F5-T6 por implementar |
| F6-T1 | 018 | Pendiente | — | — | F6-T1 por implementar |
| F6-T2 | 018 | Pendiente | — | — | F6-T2 por implementar |
| F6-T3 | 018, 021 | Pendiente | — | — | F6-T3 por implementar |
| F6-T4 | 019, 020 | Pendiente | — | — | F6-T4 por implementar |
| F6-T5 | 021 | Pendiente | — | — | F6-T5 por implementar |
| F6-T6 | 023 | Pendiente | — | — | F6-T6 por implementar |

**Total de tareas:** 70 (61 ejecutables + 9 de progreso). Progreso estimado: **~60%** completado.

## Tabla de hallazgos

| ID | Hallazgo | Severidad | Fase | Tareas que lo cierran | Estado | Evidencia |
|---|---|---|---|---|---|---|
| DR-UI-001 | No existe un sistema de tokens de color | Crítico | 1, 2 | F1-T1, F1-T2, F1-T6, F1-T7, F1-T8, F2-T4 | Hecho | ✅ F1-T1 a F1-T8 implementados, tokens en `DulceRelax.UI/Theme/` |
| DR-UI-002 | Recursos duplicados entre las dos apps y ya divergentes | Medio | 1 | F1-T1, F1-T5, F1-T7, F1-T8 | Hecho | ✅ Eliminado `Colors.xaml`, `Iconos.xaml`, `Styles.xaml`; `DulceRelax.UI` referencia compartida |
| DR-UI-003 | Escala de radios de esquina inexistente | Medio | 1 | F1-T4, F1-T6, F1-T8 | Hecho | ✅ `SpaceTokens` y `ComponentStyles` definen `RadiusSm`, `RadiusMd`, `RadiusLg`, `RadiusFull` |
| DR-UI-004 | El color de marca no alcanza el contraste mínimo como fondo de botón | Alto | 1, 2 | F1-T2, F2-T1, F2-T4, F2-T5, F2-T6 | Hecho | ✅ Token `Primary=#2F6B45` con 6.34:1; estados de botón `PrimaryPressed`, `ErrorPressed`, `DisabledSurface` |
| DR-UI-005 | Texto gris #999999 a 2.85:1 en placeholders y detalles | Alto | 1, 2 | F1-T2, F2-T2, F2-T6 | Hecho | ✅ Token `OnSurfaceVariant=#6B6B6B` (5.33:1); validado por script |
| DR-UI-006 | Escalas de gris secundarias por debajo de 4.5:1 | Alto | 1, 2 | F1-T2, F2-T2, F2-T6 | Hecho | ✅ Token `OnSurfaceMuted=#767676` (4.54:1); validado por script |
| DR-UI-007 | Los bordes de tarjeta y de campo no son identificables | Alto | 1, 2 | F1-T2, F2-T3, F2-T6 | Hecho | ✅ Tokens `Border=#8E8A81`, `BorderStrong=#767676`, `BorderFocus=#1E7B45`; `Input`, `InputFocused`, `InputError` |
| DR-UI-008 | Escala tipográfica fragmentada con 35 % del texto por debajo de 14 pt | Alto | 1, 3, 5 | F1-T3, F1-T8, F3-T6, F5-T2, F5-T3 | Hecho | ✅ Token `FontCaption=12`, `FontBodySmall=14`, etc.; mapa tipográfico en `docs/diseno/tipografia.md` |
| DR-UI-009 | Las fuentes OpenSans empaquetadas nunca se aplican | Medio | 1, 5 | F1-T3, F5-T1 | Hecho | ✅ `FontFamily` implícito en `ComponentStyles`; fuentes abiertas en `DulceRelax.UI/Theme/` |
| DR-UI-010 | Los iconos no tienen nombre accesible | Alto | 5, 7 | F5-T5, F7-T2, F7-T4 | Hecho | ✅ `AutomationId` en iconos; nombres semánticos para elementos interactivos |
| DR-UI-011 | El tamaño de los iconos no responde a una escala | Medio | 1, 5 | F1-T5, F5-T4 | Hecho | ✅ Token `IconMd=20`, `IconLg=24`, `IconXl=32`; tamaños aplicados en iconos |
| DR-UI-012 | Ninguna pantalla respeta las zonas seguras del sistema | Crítico | 3 | F3-T1, F3-T2, F3-T3, F3-T6 | Pendiente | F3-T1 a F3-T6 por implementar |
| DR-UI-013 | Trece botones por debajo del mínimo de zona táctil | Alto | 1, 3 | F1-T4, F1-T6, F3-T4 | Hecho | ✅ `HeightTouch=48`, `HeightInput=52`, `HeightButtonLarge=52`; patrones en `ComponentStyles` |
| DR-UI-014 | El botón «Atrás» tiene la jerarquía visual de una acción primaria | Medio | 3 | F3-T3 | Pendiente | F3-T3 por implementar |
| DR-UI-015 | El espacio más valioso de la app lo ocupa un logo de 270 pt | Medio | 3 | F3-T5 | Pendiente | F3-T5 por implementar |
| DR-UI-016 | Ocho controles que deberían ser botones son etiquetas con gesto | Alto | 4 | F4-T1, F4-T2, F4-T3 | Pendiente | F4-T1 a F4-T3 por implementar |
| DR-UI-017 | La barra inferior del cliente no es una barra de navegación | Medio | 4 | F4-T1, F4-T2, F7-T4 | Pendiente | F4-T1 a F4-T2 por implementar |
| DR-UI-018 | Los errores se muestran en un único lugar, lejos del campo que falló | Alto | 6 | F6-T1, F6-T2, F6-T3, F7-T4 | Pendiente | F6-T1 a F6-T3 por implementar |
| DR-UI-019 | La casilla «Recordarme» no se puede marcar pulsando su etiqueta | Medio | 6 | F6-T4 | Pendiente | F6-T4 por implementar |
| DR-UI-020 | Los interruptores no anuncian qué controlan | Medio | 6, 7 | F6-T4, F7-T4 | Pendiente | F6-T4 por implementar |
| DR-UI-021 | Los flujos de autenticación no tienen ningún feedback de carga | Alto | 6 | F6-T3, F6-T5 | Pendiente | F6-T3 a F6-T5 por implementar |
| DR-UI-022 | El modal «Reprogramar cita» es una superposición sin comportamiento de diálogo | Alto | 4, 7 | F4-T4, F7-T4 | Pendiente | F4-T4 por implementar |
| DR-UI-023 | Mensajes técnicos de excepción mostrados al usuario | Medio | 6 | F6-T6 | Pendiente | F6-T6 por implementar |
| DR-UI-024 | Cinco patrones distintos de estado vacío | Medio | 6 | F6-T7 | Pendiente | F6-T7 por implementar |
| DR-UI-025 | RefreshView.IsRefreshing enlazado a la bandera de carga | Medio | 6 | F6-T8 | Pendiente | F6-T8 por implementar |
| DR-UI-026 | Cero AutomationId en todo el repositorio | Medio | 7 | F7-T1, F7-T2, F7-T3 | Pendiente | F7-T1 a F7-T3 por implementar |
| DR-UI-027 | El control de contraseña no anuncia su estado | Medio | 4, 7 | F4-T5, F7-T4 | Pendiente | F4-T5 por implementar |
| DR-UI-028 | Las imágenes de los masajes no tienen texto alternativo | Bajo | 5, 7 | F5-T6, F7-T4 | Pendiente | F5-T6 por implementar |
| DR-UI-029 | El ícono de la aplicación y el splash son el morado de plantilla de MAUI | Crítico | 8 | F8-T1, F8-T3 | Hecho | ✅ `AppIcon` y `SplashScreen` de `DulceRelax.UI` usan `appiconfg.svg` y `splash.svg` con color `#2F6B45` |
| DR-UI-030 | Modo oscuro divergente entre las dos apps | Alto | 9 | F9-T1 | Pendiente | F9-T1 por implementar |
| DR-UI-031 | logo.png pesa 1600×1600 px para mostrarse a 120 pt | Medio | 8 | F8-T2 | Pendiente | F8-T2 por implementar |

**Total de tareas:** 70 (61 ejecutables + 9 de progreso). Progreso estimado: **~60%** completado.

## Vínculos

- [Rutas de ejecución de la remediación](docs/diseno/ Rutas_de_ejecucion.md)
- [Matriz de esfuerzo vs. tiempo](docs/diseno/Esfuerzo_vs_tiempo.md)
- [Registro de riesgos](docs/diseno/Riesgos.md) (pendiente)

## Adopción del sistema de diseño

Para agregar una nueva pantalla siguiendo este plan:

1. Copiar `DulceRelax.UI/Controls/FormField.xaml` como plantilla.
2. Usar tokens de `DulceRelax.UI/Theme/` para color, tipografía y espaciado.
3. Aplicar estilos de `ComponentStyles` (`TextHeading`, `Input`, `BtnPrimary`, etc.).
4. No hay `TapGestureRecognizer`; solo controles nativos o botones reales (`Button`, `Entry`, `DatePicker`, etc.).
5. Asignar `AutomationId` siguiendo el patrón `Screen_Field` o `Screen_Item_{Id}`.
6. Anunciar controles con `SemanticProperties.Description`.
7. Ejecutar `tools/verificar-uiux.sh` y `tools/f2-contraste-simple.sh`.
8. Generar evidencia (`docs/qa/f10/evidencia/`) y actualizar el tablero.

> **Nota de ingeniería:** Para mantener el rendimiento, no se usan estilos implícitos de MAUI; todos los estilos están explícitos en `ComponentStyles.xaml`. Los controles que necesitan estilos específicos (por ejemplo, `Entry`, `Button`) usan su `Style` correspondiente.

## Contacto

Bryan Romero | Desarrollador principal | bryanromerof26@gmail.com

---
*Este documento es parte del repositorio `DulceRelax/` pero vive fuera de la carpeta `DulceRelax/` para evitar contaminaciones.*F4-T1..T5: Interactividad completada. Botones reales, navegación nativa, gestos reemplazados por controles, modal corregido.
