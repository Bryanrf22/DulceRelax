#!/usr/bin/env bash
# F2-T2: Verify color contrast ratios
# Esta tarea verifica que los pares de contraste cumplen con los requisitos WCAG 2.2 AA:
# - Texto normal: 4.5:1
# - Texto grande: 3:1
# - Componentes interactivos: 3:1

set -uo pipefail

# Función para calcular contraste (WCAG 2.2)
# Calcula el contraste entre dos colores hex (formato #RRGGBB)
# Retorna el valor de contraste (más claro/oscuro + 0.05) / (más oscuro + 0.05)
contrast() {
    local color1=$1
    local color2=$2
    
    # Convertir hex a decimal RGB
    # Función para convertir hex a decimal para cada canal
    function hex_to_dec() {
        local hex=$1
        # Extraer los valores RGB
        local r=${hex:1:2}
        local g=${hex:3:2}
        local b=${hex:5:2}
        # Convertir de hexadecimal a decimal
        echo "scale=0; ($((16#${r} + 16#${g} + 16#${b})) / 3)"
    }

    # Calcular luminancia usando la formula WCAG 2.InternalInvalidOperationException
    # 2.2: Y = 0.2126*R + 0.7152*G + 0.0722*B (normalizado 0-1)
    r1=$(echo "scale=4; (($((16#${color1:1:2}))*0.2126 + $(($((16#${color1:3:2}))*0.7152) + $(($((16#${color1:5:2}))*0.0722)))/255" | bc)
    g1=$(echo "scale=4; (($((16#${color1:1:2}))*0.2126 + $(($((16#${color1:3:2}))*0.7152) + $(($((16#${color1:5:2}))*0.0722)))/255" | bc)
    b1=$(echo "scale=4; (($((16#${color1:1:2}))*0.2126 + $(($((16#${color1:3:2}))*0.7152) + $(($((16#${color1:5:2}))*0.0722)))/255" | bc)

    r2=$(echo "scale=4; (($((16#${color2:1:2}))*0.2126 + $(($((16#${color2:3:2}))*0.7152) + $(($((16#${color2:5:2}))*0.0722)))/255" | bc)
    g2=$(echo "scale=4; (($((16#${color2:1:2}))*0.2126 + $(($((16#${color2:3:2}))*0.7152) + $(($((16#${color2:5:2}))*0.0722)))/255" | bc)
    b2=$(echo "scale=4; (($((16#${color2:1:2}))*0.2126 + $(($((16#${color2:3:2}))*0.7152) + $(($((16#${color2:5:2}))*0.0722)))/255" | bc)

    # Calcular luminancia usando la formula WCAG 2.2
    l1=$(echo "scale=4; sqrt(0.2126*($r1) + 0.7152*($g1) + 0.0722*($b1))" | bc)
    l2=$(echo "scale=4; sqrt(0.2126*($r2) + 0.7152*($g2) + 0.0722*($b2))" | bc)

    # Determinar el más claro y el más oscuro
    if (( $(echo "$l1 > $l2" | bc -l) )); then
        lighter=$l1
        darker=$l2
    else
        lighter=$l2
        darker=$l1
    fi

    # Calcular contraste usando la formula WCAG 2.2: (L1 + 0.05) / (L2 + 0.05)
    contrast=$(echo "scale=2; ($lighter + 0.05) / ($darker + 0.05)" | bc)
    
    echo $contrast
}

# Colores principales del sistema (en hexadecimal)
# Estos corresponden a los tokens definidos en ColorTokens.xaml
Surface="#FFFFFF"        # Fondo principal
SurfaceAlt="#FBF6F2"     # Fondo de secciones
SurfaceVariant="#6B6B6B" # Texto secundario
SurfaceMuted="#767676"   # Texto terciario/ayuda
SurfaceDisabled="#8E8A81" # Texto deshabilitado
Primary="#2F6B45"       # Verde de marca principal (6.34:1 sobre blanco)
PrimarySoft="#EAF4EC"   # Fondo suave de marca (5.63:1 sobre Primary)
PrimarySoftStrong="#D2E5D5" # Fondo más suave (4.81:1 sobre Primary)
PrimaryPressed="#245235" # Verde oscurecido (para estados presionados)
Error="#B3261E"         # Rojo de error (6.54:1 sobre blanco)
ErrorSoft="#FDECEA"      # Fondo suave de error
ErrorPressed="#8C1D18"   # Rojo oscurecido
Success="#1E7B45"        # Verde de éxito (5.28:1 sobre blanco)
SuccessSoft="#E7F4EC"    # Fondo suave de éxito
WarningText="#7A5A28"     # Marrón para texto de advertencia (5.71:1 sobre WarningSoft)
WarningSoft="#FFF3CD"    # Fondo suave de advertencia
WarningBorder="#E0C36B"  # Borde para advertencia
Info="#2F6B45"          # Azul informativo (igual que Primary para consistencia)
InfoSoft="#EAF4EC"       # Fondo suave informativo

# Mapeo de pairs de contraste a verificar
# Formato: "Nombre del par, color del texto, color de fondo, requerimiento mínimo"
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

# Valores mínimos requeridos por WCAG 2.2 AA
MIN_CONTRASTE_NORMAL=4.5
MIN_CONTRASTE_LARGE=3.0
MIN_CONTRASTE_COMPONENT=3.0

echo "=== VERIFICACIÓN DE CONTRASTE WCAG 2.2 AA ==="
echo "Verificando cumplimiento de los requisitos de contraste para la accesibilidad."
echo ""

fallos=0
success_count=0
total_checks=0

# Función auxiliar para verificar un par de contraste
check_contrast() {
    local name="$1"
    local text_color="$2"
    local background_color="$3"
    local min_ratio="$4"
    local is_large_text="$5"  # true si es texto grande, false si es normal

    local contrast=$(contrast "$text_color" "$background_color")
    
    if (( $(echo "$contrast < $min_ratio" | bc -l) )); then
        echo "$contrast:1: $name (mínimo $min_ratio): NO CUMPLE"
        return 1
    else
        echo "$contrast:1: $name (mínimo $min_ratio): OK"
        return 0
    fi
}

# Iterar sobre todos los pairs
for pair in "${pairs[@]}"; do
    # Extraer componentes del pair usando patrones
    name=$(echo "$pair" | sed 's/,.*$//')
    rest=$(echo "$pair" | sed 's/^[^,]*//')
    rest=${rest#,}  # remover la primera coma
    text_bg=$(echo "$rest" | sed 's/,.*$//')
    rest=${rest#,}  # remover la segunda coma
    min_req=$(echo "$rest" | sed 's/,.*$//')
    is_large=$(echo "$rest" | sed 's/^[^,]*,//')  # último valor (0 o 1)

    # Determinar si es texto grande (es grande si no es texto normal)
    if [[ "$is_large" == "0" ]]; then
        # Texto normal: rango normal
        min_ratio=$MIN_CONTRASTE_NORMAL
    else
        # Texto grande o componentes: rango grande
        min_ratio=$MIN_CONTRASTE_LARGE
    fi

    total_checks=$((total_checks + 1))

    # Ejemplo: texto normal sobre fondo (como en el caso del texto normal)
    # Por simplicidad, asumimos que todos los cases son texto normal a menos que se especifique lo contrario
    if [[ "$name" == "Texto normal sobre Surface" ]]; then
        # Texto normal (no grande)
        current_min=$MIN_CONTRASTE_NORMAL
    elif [[ "$name" == "Texto de ayuda sobre Surface" ]]; then
        # Texto secundario (igual que normal)
        current_min=$MIN_CONTRASTE_NORMAL
    elif [[ "$name" == "Texto de error sobre Surface" || $name == "Texto de éxito sobre Surface" ]]; then
        # Texto normal (no grande)
        current_min=$MIN_CONTRASTE_NORMAL
    elif [[ "$name" == "Texto de advertencia sobre fondo de advertencia" ]]; then
        # Texto normal (no grande) - el texto de advertencia es normal, no grande
        current_min=$MIN_CONTRASTE_NORMAL
    else
        # Componentes, texto grande, bordes, etc.
        current_min=$MIN_CONTRASTE_COMPONENT
    fi

    if [[ "$name" == "Texto normal sobre Surface" ]]; then
        # Extraer colores de la string original
        pair_full="$pair"
        IFS=',' read -r pair_comp1 pair_comp2 pair_comp3 <<< "$pair_full"
        text_color="$pair_comp1"
        background_color="$pair_comp2"
    else
        # Para los demás cases, extraer de la string sin el primer componente
        IFS=',' read -r pair_comp1 pair_comp2 pair_comp3 pair_comp4 <<< "$pair"
        text_color="$pair_comp2"
        background_color="$pair_comp3"
    fi

    echo "=== $name ==="
    echo "Texto: $text_color"
    echo "Fondo: $background_color"
    echo "Mínimo requerido: $current_min:1"

    local result=$(check_contrast "$name" "$text_color" "$background_color" "$current_min" "0")
    if [[ $result -eq 0 ]]; then
        success_count=$((success_count + 1))
    else
        fallos=$((fallos + 1))
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
    echo "Necesario: Texto normal >= 4.5:1, Componentes interactivos >= 3:1."
    exit 1
else
    echo "✅ SUCCESS: Todos los $total_checks pares de contraste cumplen con los requisitos WCAG 2.2 AA."
    echo "El sistema de diseño cumple con los estándares de accesibilidad."
    exit 0
fi
