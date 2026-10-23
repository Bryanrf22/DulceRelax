#!/usr/bin/env bash
# Apply SafeAreaBehavior to all ContentPage elements in XAML files.
# Also generates a cleaned padding report.

set -euo pipefail

BASE_DIR="$(cd "$(dirname "$0")/.." && pwd)"
CLIENT_DIR="$BASE_DIR/DulceRelax.App.Cliente/Views"
ADMIN_DIR="$BASE_DIR/DulceRelax.App.Admin/Views"

# Patterns to identify ContentPage elements
CONTENT_PAGE_OPEN='<ContentPage'
CONTENT_PAGE_CLOSE='</ContentPage>'

apply_safearea() {
    local file="$1"
    local count=0
    
    # Check if file has ContentPage elements
    if ! grep -q '<ContentPage' "$file"; then
        echo "SKIP: $file (no ContentPage)"
        return 0
    fi
    
    # If Behaviors already exists, skip
    if grep -q 'SafeAreaBehavior' "$file"; then
        echo "SKIP: $file (already has SafeAreaBehavior)"
        return 0
    fi
    
    # Add Behaviors inside the opening ContentPage tag
    # We need to insert before the closing > of ContentPage
    sed -i -e '/<ContentPage/{
        s/\(/>$/\n            <Behaviors>\n                <behaviors:SafeAreaBehavior Edges="All" />\n            </Behaviors>/
    }' "$file"
    
    count=$(grep -c 'SafeAreaBehavior' "$file")
    echo "APPLIED: $file ($count occurrence(s))"
}

for dir in "$CLIENT_DIR" "$ADMIN_DIR"; do
    for f in "$dir"/*.xaml; do
        [ -f "$f" ] || continue
        apply_safearea "$f"
    done
done

echo ""
echo "Done applying SafeAreaBehavior."
