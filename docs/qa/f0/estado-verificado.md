# Estado verificado de cada fase (solo los que se CONFIRMAN con archivo)

## Completados (cambio en archivo verificable)
- F0-T1: tag `auditoria-uiux-4.0`; rama `remediacion/uiux`
- F0-T4: tablero `docs/qa/f0/tablero.md`
- F1-T1 a F1-T7: `DulceRelax.UI/` creado, tokens, ComponentStyles, App.xaml merge, recursos viejos eliminados
- F2-T1: `ColorTokens.xaml` `#2F6B45`; `f2-t1-verify-brand-color.sh`
- F2-T2: `f2-contraste-simple.sh` → 0 fallos (11/11 pasan WCAG 2.2 AA)
- F3-T1: `SafeAreaBehavior.cs` creado
- F3-T2: 15 archivos `.xaml` con `<ContentPage.Behaviors>` y `xmlns:behaviors`
- F3-T5: `PrincipalCliente.xaml` logo 270→120; padding `32,40,32,24`

## Parciales / documentados pero NO verificados en archivos
- F4-T1 a F4-T5: COMPLETADOS (verificados con archivo)
  - T1: 0 `TapGestureRecognizer`; `Button` (`IconButton`) en Registro/InicioAdmin; `PrincipalCliente` sin gestos (TabBar)
  - T2: `LoginViewModel` / `RegistroViewModel` → `//Principal/Inicio`; `AppShell` rutas actualizadas; `NavBarIsVisible="False"`
  - T3: 6/8 pseudo-botones (3 navegación cubiertos por TabBar + 3 enlaces/ojos como `Button`; `InicioSesionCliente` reconstruido con `BtnLink`)
  - T4: `PrincipalAdmin.xaml` diálogo `MostrandoReprogramar` con `Border`, `Overlay`, `AutomationId="reprogramar_modal"`, `SemanticProperties`, `DatePicker`/`TimePicker`
  - T5: `OnTogglePassword` actualiza `SemanticProperties.Description` dinámicamente en Registro/InicioAdmin/InicioCliente
- F4-T2 a F4-T5: verificados (NavBar False, navegación corregida, modal corregido) pero sin cambio de archivo amplio
- F5-T1 a F5-T4: tokens existen; sin migración completa a vistas (aún 200 font sizes restantes)
- F5-T5 / F5-T6: sin cambios en archivos de vista (solo identificados con grep)

## Bloqueado / no iniciado
- F6-T1 a F6-T8: sin cambios
- F7-T1 a F7-T4: `AutomationId` no aplicado a vistas; sin `Appium`
- F8-T1 a F8-T3: icono/splash aún plantilla `.NET`; `logo.png` sin optimizar
- F9-T1: `AppTheme.Light` en cliente, falta admin; documento `tema.md` no creado
- F10-T1 a T7: sin evidencia de cierre

## Nota de ingeniería
No marcar como completo lo que no tiene evidencia de archivo. El usuario confirmó que la confirmación debe ser solo entre fases grandes; dentro de F3, F4, F5 se avanza sin pedir, pero se debe verificar antes de declarar terminado.
