#!/usr/bin/env bash
# Línea base de compilación (F0-T1).
#
# En Linux solo se puede compilar Android: el workload de iOS no existe para
# este sistema operativo. Además, `dotnet build -f net9.0-android` intenta
# restaurar también net9.0-ios, por lo que hay que limitar TargetFrameworks en
# la restauración y compilar las referencias de proyecto por separado.
#
# Uso:
#   export PATH=/home/bryan/.dotnet9:$PATH   # SDK 9 con workload maui-android
#   bash docs/qa/f0/build-baseline.sh
#
# El resultado se escribe en docs/qa/f0/build-baseline.txt.

set -uo pipefail
cd "$(dirname "$0")/../../.."

salida="docs/qa/f0/build-baseline.txt"
: > "$salida"

registrar() {
    local nombre="$1"
    echo "===== $nombre =====" | tee -a "$salida"
    shift
    "$@" 2>&1 | tee -a "$salida"
    local estado=${PIPESTATUS[0]}
    echo "===== fin: $nombre (exit $estado) =====" | tee -a "$salida"
    echo | tee -a "$salida"
    return "$estado"
}

# API y Shared: net9.0 puro.
registrar "clean API" dotnet clean DulceRelax.API/DulceRelax.API.csproj -c Debug
registrar "API (net9.0)" \
    dotnet build DulceRelax.API/DulceRelax.API.csproj -c Debug

# Referencias compartidas para las apps (net9.0).
registrar "clean Shared" dotnet clean DulceRelax.Shared/DulceRelax.Shared.csproj -c Debug
registrar "Shared (net9.0)" \
    dotnet build DulceRelax.Shared/DulceRelax.Shared.csproj -c Debug

compilar_app_android() {
    local proyecto="$1"
    dotnet restore "$proyecto" -p:TargetFrameworks=net9.0-android -p:RestoreRecursive=false
    # `dotnet clean` de la app también limpia las referencias de proyecto.
    dotnet clean "$proyecto" -c Debug -f net9.0-android \
        -p:TargetFrameworks=net9.0-android
    # Se recompila Shared para que exista su salida net9.0 antes de compilar
    # la app con BuildProjectReferences=false.
    dotnet build DulceRelax.Shared/DulceRelax.Shared.csproj -c Debug
    dotnet build "$proyecto" -c Debug -f net9.0-android \
        -p:TargetFrameworks=net9.0-android --no-restore \
        -p:BuildProjectReferences=false
}

registrar "App Cliente (net9.0-android)" \
    compilar_app_android DulceRelax.App.Cliente/DulceRelax.App.Cliente.csproj

registrar "App Admin (net9.0-android)" \
    compilar_app_android DulceRelax.App.Admin/DulceRelax.App.Admin.csproj

echo "Errores encontrados en el log:" | tee -a "$salida"
grep -c " error " "$salida" | tee -a "$salida" || true
