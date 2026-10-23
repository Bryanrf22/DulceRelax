# F0-T5 · Guion de verificación manual reutilizable

**Tarea:** F0-T5
**Uso:** se ejecuta completo en Fase 10 y por muestreo al cerrar cada fase.
**Regla:** cada ítem tiene resultado **PASA / NO PASA**. Un ítem sin evidencia cuenta como NO PASA.

## 1. Preparación

1. Instalar la build de Release en el dispositivo de la matriz (`docs/qa/f0/matriz-dispositivos.md`).
2. Tener a mano: `adb` (Android), Xcode Simulator (iOS), TalkBack/VoiceOver, y un medidor de contraste.
3. Crear la carpeta de evidencia de la fase: `docs/qa/f10/evidencia/`.
4. Anotar en cada captura: pantalla, plataforma, versión del SO, tema del sistema y tamaño de fuente.
5. Datos de siembra: un usuario cliente y un usuario administrador. **No** se guardan credenciales en el repositorio.

## 2. Verificación automática

Ejecutar desde `DulceRelax/`:

```bash
bash tools/verificar-uiux.sh          # Anexo C.1
python3 tools/contraste.py            # Anexo C.2
```

| # | Comprobación | Comando | Criterio | Resultado |
|---|---|---|---|---|
| A1 | Sin colores literales fuera de tokens | `grep -rnE '#[0-9A-Fa-f]{6}' --include=*.xaml . \| grep -v 'DulceRelax.UI/Theme/ColorTokens.xaml' \| grep -v 'Platforms/Android/Resources/values/colors.xml'` | 0 líneas | — |
| A2 | Sin `Color.FromArgb` | `grep -rn 'Color.FromArgb' --include=*.cs .` | 0 líneas | — |
| A3 | Sin `FontSize` numéricos en vistas | `grep -rnE 'FontSize="[0-9]' --include=*.xaml . \| grep -v 'DulceRelax.UI/Theme/'` | 0 líneas | — |
| A4 | Sin `CornerRadius` numéricos en vistas | `grep -rnE 'CornerRadius="[0-9]' --include=*.xaml . \| grep -v 'DulceRelax.UI/Theme/'` | 0 líneas | — |
| A5 | Sin controles menores de 44 dp | `grep -rnE 'HeightRequest="(3[0-9]\|4[0-3])"' --include=*.xaml .` | 0 líneas | — |
| A6 | Sin pseudo-botones | `grep -rn 'TapGestureRecognizer' --include=*.xaml .` | 0 líneas | — |
| A7 | Sin botones «Atrás» manuales | `grep -rn 'OnBackClicked' --include=*.xaml --include=*.cs .` | 0 líneas | — |
| A8 | Sin colores por nombre | `grep -rn 'TextColor="Red"' --include=*.xaml .` | 0 líneas | — |
| A9 | Sin colores de plantilla MAUI | `grep -rnE '#(512BD4\|2B0B98\|ac99ea\|DFD8F7)' --include=*.xaml --include=*.xml .` | 0 líneas | — |
| A10 | `AutomationId` suficientes | `grep -rho 'AutomationId=' --include=*.xaml . \| wc -l` | ≥ 60 | — |
| A11 | `SafeAreaBehavior` aplicado | `grep -rl 'SafeAreaBehavior' --include=*.cs DulceRelax.App.Cliente DulceRelax.App.Admin \| wc -l` | ≥ 12 | — |
| A12 | Build Android limpio | `docs/qa/f0/build-baseline.sh` (adaptado a Release en F10) | 0 errores | — |
| A13 | Conteo de controles interactivos | Anexo C.3 | `AutomationId` ≥ 60 y `SemanticProperties.*` ≥ 40 | — |

## 3. Recorrido por las 15 pantallas

Marcar `PASA` solo si **todas** las comprobaciones de la fila se cumplen en Android **y** en iOS.

| # | Pantalla | Qué se comprueba | Resultado Android | Resultado iOS |
|---|---|---|---|---|
| 1 | Cliente · Login | Sin contenido bajo la barra de estado; logo a 120 dp; campos con borde visible; ojo anuncia «Mostrar contraseña»; «Recordarme» se activa por el switch; enlace de recuperación es botón; enlace de registro es botón; error bajo el campo; spinner al enviar | — | — |
| 2 | Cliente · Registro | 5 campos con etiqueta y error individual; scroll con teclado en iPhone SE; resumen de error anunciado y foco al primer campo inválido; botón deshabilitado mientras envía | — | — |
| 3 | Cliente · Recuperar contraseña | Campo de correo con validación; mensaje de éxito/error con color y `IsVisible`; botón deshabilitado al enviar | — | — |
| 4 | Cliente · Inicio | Logo 72 dp; saludo con el primer nombre; catálogo visible sin scroll en iPhone SE; estado vacío útil; pull-to-refresh no gira al abrir; 3 pestañas nativas | — | — |
| 5 | Cliente · Reserva | Barra nativa con «Solicitar cita» y flecha; fecha con 5 días de anticipación; hora dentro del horario real; errores por campo; teclado no tapa el botón | — | — |
| 6 | Cliente · Mis citas | Pestaña seleccionada anunciada; badges con contraste; acciones de 48 dp; editar/cancelar con contexto; estado vacío con acción | — | — |
| 7 | Cliente · Perfil | Avatar anunciado con el nombre; editar/cambiar contraseña/cerrar sesión con 48 dp; logout vuelve al login sin tab bar | — | — |
| 8 | Cliente · Editar perfil | Barra nativa; campos con `Input`; validación por campo; guardar vuelve a la pestaña Perfil | — | — |
| 9 | Admin · Login | Igual que el login cliente; «Recordarme» con switch; sin barra de tab; tema claro forzado | — | — |
| 10 | Admin · Agenda | Navegación de días con botones de 48 dp; badges verde/ámbar/rojo; acciones en `FlexLayout` sin desbordar en 360 dp; diálogo de reprogramación anunciado, enfocado y cerrable con «Atrás» | — | — |
| 11 | Admin · Solicitudes | Acciones confirmar/rechazar de 48 dp; estado vacío útil; sin desbordes | — | — |
| 12 | Admin · Masajes | Botón «Nuevo» de 48 dp; badge y acción con tonos opuestos; editar/activar con contexto; estado vacío con acción | — | — |
| 13 | Admin · Formulario de masaje | Barra nativa con título dinámico; validación de precio/duración/URL; `Editor` con `AutoSize`; guardar de 48 dp | — | — |
| 14 | Admin · Clientes | Botón «Llamar» de 48 dp; estado vacío; datos legibles a 14+ | — | — |
| 15 | Admin · Ajustes | 7 switches anuncian su día; error bajo el día inválido; guardar de 48 dp; tema claro | — | — |

## 4. Guion de lector de pantalla

Ejecutar **sin mirar la pantalla**, primero con TalkBack (Android) y luego con VoiceOver (iOS).

| # | Acción | Resultado esperado | Hallazgo | Resultado Android | Resultado iOS |
|---|---|---|---|---|---|
| 1 | Abrir login | Se lee el título y luego el campo de correo con su etiqueta | 010 | — | — |
| 2 | Pulsar el ojo | «Mostrar contraseña»; al pulsar, «Ocultar contraseña» y anuncia «Contraseña visible» | 027 | — | — |
| 3 | Enviar vacío | Se anuncia el resumen y el foco va al primer campo | 018 | — | — |
| 4 | Abrir el catálogo | «Hola, {nombre}» y cada masaje con nombre y precio; las imágenes no se anuncian | 028 | — | — |
| 5 | Navegar a Mis citas | La pestaña se anuncia como seleccionada | 017 | — | — |
| 6 | Abrir una cita | Estado, fecha, dirección y acciones con contexto | 010 | — | — |
| 7 | Admin: agenda | Fecha, cada cita y sus acciones | 010 | — | — |
| 8 | Admin: ajustes | Cada interruptor dice su día; al guardar inválido se anuncia el día | 020 | — | — |
| 9 | Admin: reprogramar | Al abrir se anuncia el diálogo; «Atrás» lo cierra | 022 | — | — |
| 10 | Fuente del sistema al máximo | Ningún texto informativo se recorta | 008 | — | — |

Criterio adicional: **0 controles** anunciados como «botón sin nombre» o con un glifo (`\ue...`).

## 5. Contraste

Medir con WebAIM, Stark o `tools/contraste.py` (Anexo C.2). Un par falla si queda por debajo del mínimo.

| # | Combinación | Mínimo | Resultado |
|---|---|---|---|
| 1 | `OnPrimary` sobre `Primary` | 4.5 | — |
| 2 | `Primary` sobre `PrimarySoft` | 4.5 | — |
| 3 | `Primary` sobre `PrimarySoftStrong` | 4.5 | — |
| 4 | `OnSurface` sobre `Surface` | 4.5 | — |
| 5 | `OnSurfaceVariant` sobre `Surface` | 4.5 | — |
| 6 | `OnSurfaceVariant` sobre `SurfaceAlt` | 4.5 | — |
| 7 | `OnSurfaceMuted` sobre `Surface` | 4.5 | — |
| 8 | `WarningText` sobre `WarningSoft` | 4.5 | — |
| 9 | `Error` sobre `Surface` | 4.5 | — |
| 10 | `Success` sobre `Surface` | 4.5 | — |
| 11 | `Border` sobre `Surface` | 3.0 | — |
| 12 | `Border` sobre `SurfaceAlt` | 3.0 | — |
| 13 | `BorderStrong` sobre `Surface` | 3.0 | — |
| 14 | `DisabledText` sobre `DisabledSurface` | exento (informativo) | — |

## 6. Estados de pantalla

Por cada pantalla con lista o formulario:

| Estado | Qué se comprueba | Resultado |
|---|---|---|
| Cargando | Se ve el spinner inicial (`ActivityIndicator`), no el de pull-to-refresh | — |
| Error | Mensaje en español del catálogo `MensajesError`, sin `ex.Message`, sin códigos HTTP | — |
| Vacío | `EmptyStateView` con título, mensaje y acción cuando aplica; **no** aparece durante la carga | — |
| Contenido | La lista o el formulario se ve sin solaparse con spinner, error ni estado vacío | — |
| Éxito | Mensaje verde con `IsVisible` ligado al estado; desaparece al reintentar | — |
| Envío | Botón deshabilitado, texto «Entrando…/Enviando…» y sin doble envío | — |

## 7. Navegación

| # | Flujo | Resultado esperado | Resultado |
|---|---|---|---|
| N1 | Cliente: login → catálogo | Aterriza en la pestaña Inicio; la tab bar no aparece en el login | — |
| N2 | Cliente: catálogo → reservar → guardar | Vuelve a la pestaña Inicio | — |
| N3 | Cliente: Mis citas → editar → guardar | Vuelve a Mis citas con el dato actualizado | — |
| N4 | Cliente: Perfil → editar → guardar | Vuelve a Perfil | — |
| N5 | Cliente: cerrar sesión | Vuelve al login y la tab bar desaparece | — |
| N6 | Cliente: gesto/botón Atrás | No hay callejones sin salida; en la pestaña raíz el sistema decide | — |
| N7 | Admin: login → agenda | Aterriza en la agenda | — |
| N8 | Admin: agenda → reprogramar → Atrás | El diálogo se cierra; la app no se cierra | — |
| N9 | Admin: solicitudes → confirmar | La cita pasa a la agenda con el estado correcto | — |
| N10 | Admin: masajes → formulario → guardar | Vuelve al listado con el masaje actualizado | — |

## 8. Matriz de los 31 hallazgos

| ID | Cómo se comprueba | Criterio PASA | Resultado | Evidencia |
|---|---|---|---|---|
| 001 | `A1` + inspección de estilos | 0 literales fuera de tokens; todos los estilos apuntan a `ColorTokens` | — | — |
| 002 | `git ls-files \| grep Resources/Styles` + arranque de ambas apps | 0 archivos duplicados; sin `XamlParseException` | — | — |
| 003 | `A4` + `grep CornerRadius` | Solo 4 radios definidos en tokens; 0 en vistas | — | — |
| 004 | Pares 1–3 de contraste + captura de botón | `Primary` = `#2F6B45`; blanco encima ≥ 4.5:1 | — | — |
| 005 | `A1` + par 7 | Ningún `#999999`; placeholder ≥ 4.5:1 | — | — |
| 006 | Pares 5–7 | Grises secundarios ≥ 4.5:1 sobre su superficie real | — | — |
| 007 | `grep 'Stroke="#'` + pares 11–13 | 0 `Stroke` literal; campos con `BorderStrong` ≥ 3:1 | — | — |
| 008 | `A3` + guion 10 | Solo 12/14/16/18/22/28; sin texto informativo < 14 | — | — |
| 009 | Captura comparativa Android/iOS | Misma forma de letra; `FontFamily` implícito en Label/Entry/Button | — | — |
| 010 | Guion 1–10 | Todo control interactivo tiene nombre; iconos decorativos ocultos | — | — |
| 011 | `A3` + `F5-T4` | Iconos solo en 16/20/24/32 | — | — |
| 012 | Recorrido 1–15 en Android 15 y iPhone con notch | Sin contenido bajo barras/notch; sin franja doble | — | — |
| 013 | `A5` | 0 botones < 48 dp | — | — |
| 014 | `A7` + recorrido 5, 8, 13 | Barra nativa con título y flecha en las 3 páginas apiladas | — | — |
| 015 | Recorrido 4 en iPhone SE | Logo a 72 dp; al menos una tarjeta completa visible | — | — |
| 016 | `A6` + guion 2 | 0 `TapGestureRecognizer`; enlaces y ojo son botones | — | — |
| 017 | Guion 5 + recorrido 4 | 3 pestañas nativas; estado de selección anunciado | — | — |
| 018 | Guion 3 + recorrido 2, 5, 8 | Error bajo el campo; foco al primer inválido; resumen anunciado | — | — |
| 019 | Recorrido 1, 9 | «Recordarme» se activa con el switch | — | — |
| 020 | Guion 8 | Cada switch anuncia su día y estado | — | — |
| 021 | Recorrido 1, 2, 3, 9 | Botón deshabilitado + texto de progreso + spinner | — | — |
| 022 | Guion 9 + N8 | Diálogo anunciado, con foco dentro, cerrable con «Atrás» y desplazable | — | — |
| 023 | `grep 'ex.Message'` + revisión de errores | 0 mensajes técnicos en UI | — | — |
| 024 | Recorrido 4, 6, 10, 11, 12, 14, 15 | `EmptyStateView` en las 7 pantallas, con acción cuando aplica | — | — |
| 025 | Recorrido 4 en carga inicial y pull-to-refresh | Sin spinner de refresh al abrir; se detiene al fallar | — | — |
| 026 | `A10` + `A13` | ≥ 60 `AutomationId`; IDs únicos por pantalla | — | — |
| 027 | Guion 2 | Descripción dinámica y anuncio de estado | — | — |
| 028 | Guion 4 | Imágenes decorativas ocultas; avatar del perfil anunciado | — | — |
| 029 | Instalación limpia + captura de lanzador/splash | Icono verde con monograma; splash crema; sin morado de plantilla | — | — |
| 030 | Captura con el sistema en claro y en oscuro | Ambas apps idénticas en oscuro; `UserAppTheme = Light`; sin `AppThemeBinding` | — | — |
| 031 | `find obj -name 'logo*.png'` + memoria | Logo no decodificado a 1600×1600; nitidez a 72/120 dp | — | — |

## 9. Cómo reportar un fallo

Copiar esta plantilla por cada incumplimiento:

```markdown
### [ID corto] Pantalla · plataforma
- **Pantalla:** (ej. Cliente · Reserva)
- **Plataforma / dispositivo:** (ej. Pixel 7, Android 15; iPhone 13, iOS 17)
- **Tema del sistema:** claro / oscuro
- **Tamaño de fuente:** normal / máximo
- **Pasos:**
  1. ...
  2. ...
- **Esperado:** ...
- **Obtenido:** ...
- **Hallazgo relacionado:** DR-UI-nnn
- **Control / AutomationId:** ...
- **Captura:** `docs/qa/f10/evidencia/nombre.png`
- **Severidad:** crítico / alto / medio / bajo
```

Reglas:

- Un fallo sin captura o sin pasos reproducibles no se acepta como evidencia de cierre.
- Los fallos que revelen un hallazgo **nuevo** (no listado en los 31) se agregan al tablero como `DR-UI-032+` y se decide si bloquean el cierre.
- Los fallos de accesibilidad se registran con el nombre del control y lo que anunció el lector.

## 10. Registro de ejecución

| Fecha | Ejecutor | Plataforma / dispositivo | Build | Ítems ejecutados | PASA | NO PASA | Observaciones |
|---|---|---|---|---|---|---|---|
| — | — | — | — | — | — | — | — |

## Criterio de aceptación

- [x] El guion cubre las 15 pantallas.
- [x] El guion cubre los 31 hallazgos.
- [x] Cada ítem tiene resultado binario (PASA / NO PASA).
- [ ] El guion fue ejecutado al menos una vez en Android y una vez en iOS.
