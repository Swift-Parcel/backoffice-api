#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
INIT_SQL="$PROJECT_ROOT/localenv/init.sql"

DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
DB_NAME="${DB_NAME:-swiftparcel}"
DB_USER="${DB_USER:-swiftparcel}"
DB_PASSWORD="${DB_PASSWORD:-swiftparcel}"

export PGPASSWORD="$DB_PASSWORD"

if [ ! -f "$INIT_SQL" ]; then
    echo "ERROR: init.sql not found at $INIT_SQL"
    exit 1
fi

echo "Dropping database '$DB_NAME'..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d postgres -c "DROP DATABASE IF EXISTS $DB_NAME;" 2>/dev/null || true

echo "Creating database '$DB_NAME'..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d postgres -c "CREATE DATABASE $DB_NAME;"

echo "Running init.sql..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f "$INIT_SQL"

echo "Done. Database '$DB_NAME' rebuilt with legacy data from init.sql."
