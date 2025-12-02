#!/usr/bin/env bash
# Generate a secure .env file for the compose stack.
# Usage:
#   ./db_env_pass.sh          # interactive if .env exists
#   ./db_env_pass.sh -f|--force  # overwrite without prompting
#   ./db_env_pass.sh -h|--help   # show this help
set -euo pipefail

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="$DIR/.env"
FORCE=0

# Parse arguments
while [ $# -gt 0 ]; do
  case "$1" in
    -f|--force) FORCE=1; shift ;;
    -h|--help) cat <<EOF
Usage: $(basename "$0") [options]
Options:
  -f, --force   Overwrite existing .env without prompting
  -h, --help    Show this help message
EOF
      exit 0
      ;;
    *) echo "Unknown option: $1" >&2; exit 1 ;;
  esac
done

# Dependencies
if ! command -v openssl >/dev/null 2>&1; then
  echo "Error: 'openssl' is required but not installed." >&2
  exit 1
fi

# If .env exists, prompt unless forced
if [ -f "$ENV_FILE" ] && [ "$FORCE" -ne 1 ]; then
  read -r -p "$ENV_FILE already exists. Overwrite? [y/N] " ans
  case "$ans" in
    [Yy]* ) ;;
    * ) echo "Aborting."; exit 0 ;;
  esac
fi

# Write to a temp file and move atomically
TMP="$(mktemp "${TMPDIR:-/tmp}/db_env.XXXXXX")"
trap 'rm -f "$TMP"' EXIT

DB_PW="$(openssl rand -base64 32)"
PG_PW="$(openssl rand -base64 32)"
REDIS_PW="$(openssl rand -base64 32)"

cat >"$TMP" <<EOF
# Auto-generated .env — do not commit to VCS
DB_PASSWORD="${DB_PW}"
PGADMIN_DEFAULT_EMAIL="admin@stationery.com"
PGADMIN_PASSWORD="${PG_PW}"
REDIS_PASSWORD="${REDIS_PW}"

# Optional overrides:
# POSTGRES_USER=stationery_admin
# POSTGRES_DB=StationeryStore
EOF

mv "$TMP" "$ENV_FILE"
chmod 600 "$ENV_FILE"
trap - EXIT

echo "Created $ENV_FILE with restricted permissions (600)."