# Decisión de tema — DulceRelax

## Decisión

Solo **tema claro** (`AppTheme.Light`) en ambas aplicaciones (`Cliente` y `Admin`). No se implementa modo oscuro en este cierre.

## Motivo

Implementar modo oscuro completo requiere duplicar todos los tokens (`ColorTokens`), agregar `AppThemeBinding` a cada estilo, revisar colores que vienen de `ViewModel` (`TonoColorConverter`), probar 15 pantallas × 2 temas × 3 plataformas y actualizar `colors.xml`. Es ~4 días adicionales y no cierra ninguno de los 31 hallazgos de la auditoría.

La auditoría acepta explícitamente esta salida: si el producto decide no soportar modo oscuro, debe documentarse y fijarse en el panel.

## Qué se hizo

- `UserAppTheme = AppTheme.Light` en `App.xaml.cs` (cliente ya lo tenía; admin se agregó).
- `AppearanceLightStatusBars = true` y `AppearanceLightNavigationBars = true` en ambas `MainActivity.cs` para que las barras del sistema se vean sobre el fondo crema.
- `AppThemeBinding` eliminado de todos los estilos (ya no existe `Styles.xaml`).
- `docs/diseno/tema.md` creado y enlazado desde `README.md`.

## Verificación

- Con el sistema en oscuro, las 15 pantallas se ven idénticas al modo claro.
- No hay texto blanco sobre fondo blanco ni tarjetas negras.

## Revisión futura

Si una encuesta muestra > 20 % de usuarios pidiendo modo oscuro, se revisará. El orden sería:

1. Duplicar tokens `ColorTokens.xaml` (`...Light` / `...Dark`).
2. `AppThemeBinding` en estilos, no en vistas.
3. Actualizar `TonoColorConverter` según `RequestedTheme`.
4. `Shell.TabBar` y `Shadow` revisados para oscuro.
5. Matriz de contraste (Anexo C.2) en ambos temas.
