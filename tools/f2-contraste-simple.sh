#!/usr/bin/env bash
# Simple script to verify WCAG 2.2 contrast ratios for brand colors
# Verifies that the key color pairs meet accessibility requirements

set -uo pipefail

# Define color pairs to test with their contrast requirements
# Format: "Name, ColorText, ColorBackground, MinContrastRatio"
pairs=(
    "Texto normal sobre Surface, #FFFFFF, #222222, 4.5"
    "Texto secundario sobre Surface, #6B6B6B, #222222, 4.5"
    "Texto terciario sobre Surface, #767676, #222222, 4.5"
    "Texto secundario sobre SurfaceAlt, #6B6B6B, #FBF6F2, 4.5"
    "Texto de ayuda sobre Surface, #767676, #222222, 4.5"
    "Texto de advertencia sobre fondo de advertencia, #7A5A28, #FFF3CD, 4.5"
    "Texto de error sobre Surface, #B3261E, #222222, 4.5"
    "Texto de éxito sobre Surface, #1E7B45, #222222, 4.5"
    "Borde sobre Surface, #8E8A81, #222222, 3.0"
    "Borde sobre SurfaceAlt, #767676, #FBF6F2, 3.0"
    "Texto deshabilitado sobre fondo deshabilitado, #6B6B6B, #E6E3DC, 3.0"
)

# Function to calculate contrast ratio (WCAG 2.2)
# Using the formula: (L1 + 0.05) / (L2 + 0.05) where L1 > L2
# L = 0.2126*R + 0.7152*G + 0.0722*B (normalized to 0-1)
contrast() {
    local hex1=$1
    local hex2=$2
    
    # Convert hex to RGB (0-255)
    local r1=$((16#${hex1:1:2}))
    local g1=$((16#${hex1:3:2}))
    local b1=$((16#${hex1:5:2}))
    
    local r2=$((16#${hex2:1:2}))
    local g2=$((16#${hex2:3:2}))
    local b2=$((16#${hex2:5:2}))
    
    # Calculate luminance (WCAG 2.2 formula)
    local l1=$(echo "scale=4; sqrt(0.2126*($r1/255) + 0.7152*($g1/255) + 0.0722*($b1/255))" | bc)
    local l2=$(echo "scale=4; sqrt(0.2126*($r2/255) + 0.7152*($g2/255) + 0.0722*($b2/255))" | bc)
    
    # Ensure l1 > l2
    local lighter=${l1}
    local darker=${l2}
    if (( $(echo "$l2 > $l1" | bc -l) )); then
        lighter=${l2}
        darker=${l1}
    fi
    
    # Calculate contrast ratio
    local contrast=$(echo "scale=2; ($lighter + 0.05) / ($darker + 0.05)" | bc)
    
    echo $contrast
}

# Values minimum required by WCAG 2.2 AA
MIN_CONTRASTE_NORMAL=4.5
MIN_CONTRSE_LARGE=3.0

echo "=== VERIFICACIÓN DE CONTRASTE WCAG 2.2 AA ==="
echo "Verificando que las combinaciones de colores cumplen con los requisitos de accesibilidad."
echo ""

fallos=0
total_checks=0

success_count=0

# Iterate through all color pairs
for pair in "${pairs[@]}"; do
    # Parse the pair components
    name=$(echo "$pair" | cut -d',' -f1)
    text_color=$(echo "$pair" | cut -d',' -f2)
    background_color=$(echo "$pair" | cut -d',' -f3)
    min_ratio=$(echo "$pair" | cut -d',' -f4)
    
    # Determine if this is normal text or large text/components
    if [[ "$name" == *"Texto normal sobre Surface"* ]] || [[ "$name" == *"Texto secundario sobre Surface"* ]] || [[ "$name" == *"Texto terciario sobre Surface"* ]] || [[ "$name" == *"Texto de error sobre Surface"* ]] || [[ "$name" == *"Texto de éxito sobre Surface"* ]] || [[ "$name" == *"Texto de advertencia sobre fondo de advertencia"* ]]; then
        current_min=$MIN_CONTRASTE_NORMAL
    else
        # Border components use large text contrast
        current_min=$MIN_CONTRSE_LARGE
    fi

    total_checks=$((total_checks + 1))
    
    echo "=== $name ==="
    echo "Texto: $text_color"
    echo "Fondo: $background_color"
    echo "Mínimo requerido: $current_min:1"
    
    contrast=$(contrast "$text_color" "$background_color")
    
    if (( $(echo "$contrast < $current_min" | bc -l) )); then
        echo "$contrast:1: NO CUMPLE (mínimo $current_min:1)"
        fallos=$((fallos + 1))
    else
        echo "$contrast:1: OK (mínimo $current_min:1)"
        success_count=$((success_count + 1))
    fi
    
    echo ""
done

echo "=== RESUMEN ==="
echo "Total verificaciones: $total_checks"
echo "Éxitos: $success_count"
echo "Fallos: $fallos"

echo ""
if [[ $fallos -gt 0 ]]; then
    echo "❌ ERROR: $fallos par(es) de contraste no cumplen los requisitos WCAG 2.2 AA."
    echo "Se necesitan al menos $MIN_CONTRASTE_NORMAL:1 para texto normal y $MIN_CONTRSE_LARGE:1 para componentes interactivos/large text."
    exit 1
else
    echo "✅ SUCCESS: Todos los $total_checks pares de contraste cumplen con los requisitos WCAG 2.2 AA."
    echo "El sistema de diseño cumple con los estándares de accesibilidad."
    exit 0
fi
