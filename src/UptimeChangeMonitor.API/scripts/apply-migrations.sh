#!/bin/bash
set -e

echo "=========================================="
echo "  UptimeChangeMonitor - Database Migrations"
echo "=========================================="

echo ""
echo "Waiting for PostgreSQL to be ready..."

until pg_isready -h postgres -U postgres; do
  echo "PostgreSQL is unavailable - sleeping"
  sleep 1
done

echo "✓ PostgreSQL is ready!"
echo ""

echo "Applying database migrations..."
echo ""

cd /src/src/UptimeChangeMonitor.API

MIGRATIONS_DIR="/src/src/UptimeChangeMonitor.Infrastructure/Migrations"
if [ -d "$MIGRATIONS_DIR" ]; then
  MIGRATION_FILES=$(find "$MIGRATIONS_DIR" -name "*.cs" -type f 2>/dev/null | wc -l)
else
  MIGRATION_FILES=0
fi

if [ ! -d "$MIGRATIONS_DIR" ] || [ "$MIGRATION_FILES" -eq 0 ]; then
  echo "⚠️  No migrations found! Creating initial migration..."
  echo "   Migrations directory: $MIGRATIONS_DIR"
  echo "   Migration files found: $MIGRATION_FILES"
  echo ""
  
  echo "Creating initial migration..."
  
  dotnet ef migrations add InitialCreate \
    --project /src/src/UptimeChangeMonitor.Infrastructure \
    --startup-project /src/src/UptimeChangeMonitor.API \
    --verbose 2>&1 | tee /tmp/migration_output.log
  
  MIGRATION_EXIT_CODE=${PIPESTATUS[0]}
  
  if [ $MIGRATION_EXIT_CODE -ne 0 ]; then
    echo "❌ Failed to create migration!"
    echo "Output:"
    cat /tmp/migration_output.log 2>/dev/null || echo "No output captured"
    exit 1
  fi
  
  echo "✓ Initial migration created successfully!"
  echo ""
fi

dotnet ef database update \
  --project /src/src/UptimeChangeMonitor.Infrastructure \
  --startup-project /src/src/UptimeChangeMonitor.API \
  --verbose 2>&1 | tee /tmp/update_output.log

EXIT_CODE=${PIPESTATUS[0]}

if [ $EXIT_CODE -ne 0 ]; then
  echo "❌ Failed to apply migrations!"
  echo "Output:"
  cat /tmp/update_output.log 2>/dev/null || echo "No output captured"
  exit 1
fi

echo ""
echo "=========================================="
echo "✓ Migrations applied successfully!"
echo "=========================================="
