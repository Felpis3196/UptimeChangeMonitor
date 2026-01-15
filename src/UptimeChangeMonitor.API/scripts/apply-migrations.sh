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

echo "PostgreSQL is ready!"
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
  echo "WARNING: No migrations found! Creating initial migration..."
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
    echo "ERROR: Failed to create migration!"
    echo "Output:"
    cat /tmp/migration_output.log 2>/dev/null || echo "No output captured"
    exit 1
  fi
  
  echo "Initial migration created successfully!"
  echo ""
fi

# Function to check if migration is already applied
check_migration_applied() {
  local migration_name=$1
  local result
  result=$(PGPASSWORD=postgres psql -h postgres -U postgres -d UptimeChangeMonitor -tAc \
    "SELECT COUNT(*) FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" = '$migration_name';" 2>/dev/null || echo "0")
  [ "$result" = "1" ]
}

# Get the latest migration name
LATEST_MIGRATION=$(find "$MIGRATIONS_DIR" -name "*.cs" -type f -name "*_*.cs" ! -name "*ModelSnapshot.cs" | sort | tail -1 | xargs basename | sed 's/\.cs$//' || echo "")

if [ -n "$LATEST_MIGRATION" ]; then
  echo "Checking if migration '$LATEST_MIGRATION' is already applied..."
  if check_migration_applied "$LATEST_MIGRATION"; then
    echo "Migration '$LATEST_MIGRATION' is already applied in the database."
    echo ""
    echo "=========================================="
    echo "Database is up to date!"
    echo "=========================================="
    exit 0
  fi
fi

echo "Applying database migrations..."
dotnet ef database update \
  --project /src/src/UptimeChangeMonitor.Infrastructure \
  --startup-project /src/src/UptimeChangeMonitor.API \
  --verbose 2>&1 | tee /tmp/update_output.log

EXIT_CODE=${PIPESTATUS[0]}

if [ $EXIT_CODE -ne 0 ]; then
  # Check if the error is about tables already existing
  if grep -q "already exists" /tmp/update_output.log 2>/dev/null; then
    echo ""
    echo "WARNING: Some tables already exist in the database."
    echo "   This might mean the migration was partially applied or tables were created manually."
    echo ""
    echo "   Attempting to mark migration as applied..."
    
    # Try to get the migration name from the output or use the latest one
    MIGRATION_TO_MARK=$(grep -oP "Applying migration '\K[^']+" /tmp/update_output.log | head -1 || echo "$LATEST_MIGRATION")
    
    if [ -n "$MIGRATION_TO_MARK" ]; then
      # Check if migration history table exists
      TABLE_EXISTS=$(PGPASSWORD=postgres psql -h postgres -U postgres -d UptimeChangeMonitor -tAc \
        "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = '__EFMigrationsHistory');" 2>/dev/null || echo "f")
      
      if [ "$TABLE_EXISTS" = "t" ]; then
        
        # Check if migration is not already registered
        if ! check_migration_applied "$MIGRATION_TO_MARK"; then
          # Get ProductVersion from the model snapshot or use default
          PRODUCT_VERSION="8.0.0"
          SNAPSHOT_FILE=$(find "$MIGRATIONS_DIR" -name "*ModelSnapshot.cs" | head -1)
          if [ -f "$SNAPSHOT_FILE" ]; then
            # Try to extract ProductVersion from snapshot (format: ProductVersion = "8.0.0")
            EXTRACTED_VERSION=$(grep -oP 'ProductVersion\s*=\s*"\K[^"]+' "$SNAPSHOT_FILE" | head -1)
            if [ -n "$EXTRACTED_VERSION" ]; then
              PRODUCT_VERSION="$EXTRACTED_VERSION"
            fi
          fi
          
          echo "   Registering migration '$MIGRATION_TO_MARK' with ProductVersion '$PRODUCT_VERSION'..."
          
          # Insert migration record
          if PGPASSWORD=postgres psql -h postgres -U postgres -d UptimeChangeMonitor -c \
            "INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('$MIGRATION_TO_MARK', '$PRODUCT_VERSION') ON CONFLICT DO NOTHING;" 2>/dev/null; then
            echo "Migration '$MIGRATION_TO_MARK' marked as applied."
            echo ""
            echo "=========================================="
            echo "Database migration status updated!"
            echo "=========================================="
            exit 0
          else
            echo "WARNING: Could not mark migration as applied automatically."
            echo "   Please check database permissions and connection."
          fi
        else
          echo "Migration '$MIGRATION_TO_MARK' is already registered."
          echo ""
          echo "=========================================="
          echo "Database is up to date!"
          echo "=========================================="
          exit 0
        fi
      else
        echo "WARNING: Migration history table does not exist."
        echo "   The database might be in an inconsistent state."
      fi
    fi
    
    echo ""
    echo "WARNING: Could not automatically resolve the migration state."
    echo "   Please check the database manually or drop and recreate it."
    echo ""
    echo "   Error details:"
    cat /tmp/update_output.log 2>/dev/null | grep -A 5 "already exists" || echo "   See full output above"
    exit 1
  else
    echo "ERROR: Failed to apply migrations!"
    echo "Output:"
    cat /tmp/update_output.log 2>/dev/null || echo "No output captured"
    exit 1
  fi
fi

echo ""
echo "=========================================="
echo "Migrations applied successfully!"
echo "=========================================="
