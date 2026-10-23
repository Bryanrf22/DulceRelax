#!/usr/bin/env python3
"""Verifica los pares de color del sistema de diseño contra WCAG 2.2.

Anexo C.2 del plan de remediación UI/UX de DulceRelax.
"""


def luminancia(hexadecimal: str) -> float:
    valor = hexadecimal.lstrip("#")
    canales = [int(valor[i:i + 2], 16) / 255 for i in (0, 2, 4)]
    lineal = [c / 12.92 if c <= 0.04045 else ((c + 0.055) / 1.055) ** 2.4 for c in canales]
    return 0.2126 * lineal[0] + 0.7152 * lineal[1] + 0.0722 * lineal[2]


def contraste(fondo: str, texto: str) -> float:
    l1, l2 = luminancia(fondo), luminancia(texto)
    claro, oscuro = max(l1, l2), min(l1, l2)
    return (claro + 0.05) / (oscuro + 0.05)


PARES = [
    ("#2F6B45", "#FFFFFF", 4.5, "OnPrimary sobre Primary"),
    ("#EAF4EC", "#2F6B45", 4.5, "Primary sobre PrimarySoft"),
    ("#D2E5D5", "#2F6B45", 4.5, "Primary sobre PrimarySoftStrong"),
    ("#FFFFFF", "#222222", 4.5, "OnSurface sobre Surface"),
    ("#FFFFFF", "#6B6B6B", 4.5, "OnSurfaceVariant sobre Surface"),
    ("#FBF6F2", "#6B6B6B", 4.5, "OnSurfaceVariant sobre SurfaceAlt"),
    ("#FFFFFF", "#767676", 4.5, "OnSurfaceMuted sobre Surface"),
    ("#FFF3CD", "#7A5A28", 4.5, "WarningText sobre WarningSoft"),
    ("#FFFFFF", "#B3261E", 4.5, "Error sobre Surface"),
    ("#FFFFFF", "#1E7B45", 4.5, "Success sobre Surface"),
    ("#FFFFFF", "#8E8A81", 3.0, "Border sobre Surface"),
    ("#FBF6F2", "#8E8A81", 3.0, "Border sobre SurfaceAlt"),
    ("#FFFFFF", "#767676", 3.0, "BorderStrong sobre Surface"),
    ("#E6E3DC", "#6B6B6B", 3.0, "DisabledText sobre DisabledSurface"),
]

fallos = 0
for fondo, texto, minimo, nombre in PARES:
    ratio = contraste(fondo, texto)
    estado = "OK " if ratio >= minimo else "FALLO"
    if ratio < minimo:
        fallos += 1
    print(f"{estado} {ratio:5.2f}:1 (min {minimo})  {nombre}")

print()
if fallos:
    raise SystemExit(f"{fallos} pares no cumplen")
print("Todos los pares cumplen.")
