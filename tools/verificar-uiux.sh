#!/usr/bin/env bash
# Verificación estática de la remediación UI/UX (Anexo C.1 del plan).
# Ejecutar antes de cada PR. Sale con código 1 si alguna comprobación falla.
set -uo pipefail
cd "$(dirname "$0")/.."

fallos=0

check() {
    local descripcion="$1"
    local salida="$2"
    if [ -n "$salida" ]; then
        echo "FALLO: $descripcion"
        echo "$salida" | head -20
        fallos=$((fallos + 1))
    else
        echo "OK: $descripcion"
    fi
}

colores=$(grep -rnE '#[0-9A-Fa-f]{6}' --include=*.xaml . \
    | grep -v 'DulceRelax.UI/Theme/ColorTokens.xaml' \
    | grep -v 'Platforms/Android/Resources/values/colors.xml' || true)
check "Sin colores literales fuera de los tokens" "$colores"

fromargb=$(grep -rn 'Color.FromArgb' --include=*.cs . || true)
check "Sin Color.FromArgb en C#" "$fromargb"

fuentes=$(grep -rnE 'FontSize="[0-9]' --include=*.xaml . \
    | grep -v 'DulceRelax.UI/Theme/' || true)
check "Sin FontSize numéricos en vistas" "$fuentes"

radios=$(grep -rnE 'CornerRadius="[0-9]' --include=*.xaml . \
    | grep -v 'DulceRelax.UI/Theme/' || true)
check "Sin CornerRadius numéricos en vistas" "$radios"

botones=$(grep -rnE 'HeightRequest="(3[0-9]|4[0-3])"' --include=*.xaml . || true)
check "Sin controles por debajo de 44 dp" "$botones"

gestos=$(grep -rn 'TapGestureRecognizer' --include=*.xaml . || true)
check "Sin pseudo-botones (TapGestureRecognizer)" "$gestos"

atras=$(grep -rn 'OnBackClicked' --include=*.xaml --include=*.cs . || true)
check "Sin botones Atrás manuales" "$atras"

rojo=$(grep -rn 'TextColor="Red"' --include=*.xaml . || true)
check "Sin colores por nombre" "$rojo"

plantilla=$(grep -rnE '#(512BD4|2B0B98|ac99ea|DFD8F7)' --include=*.xaml --include=*.xml . \
    | grep -v '/obj/' || true)
check "Sin colores de plantilla MAUI" "$plantilla"

ids=$(grep -rho 'AutomationId=' --include=*.xaml . | wc -l)
if [ "$ids" -lt 60 ]; then
    echo "FALLO: AutomationId insuficientes ($ids < 60)"
    fallos=$((fallos + 1))
else
    echo "OK: AutomationId ($ids)"
fi

behaviors=$(grep -rl 'SafeAreaBehavior' --include=*.cs DulceRelax.App.Cliente DulceRelax.App.Admin 2>/dev/null | wc -l)
if [ "$behaviors" -lt 12 ]; then
    echo "FALLO: SafeAreaBehavior aplicado en $behaviors páginas (< 12)"
    fallos=$((fallos + 1))
else
    echo "OK: SafeAreaBehavior en $behaviors páginas"
fi

echo
if [ "$fallos" -gt 0 ]; then
    echo "Resultado: $fallos comprobaciones fallidas."
    exit 1
fi

echo "Resultado: todas las comprobaciones pasaron."
