#!/usr/bin/env bash
# Uso: ./setup.sh <usuario> [senha]
# Gera o arquivo de senhas do Mosquitto.
set -euo pipefail

USER_NAME="${1:?Uso: ./setup.sh <usuario> [senha]}"
if [ -n "${2:-}" ]; then
  PASSWORD="$2"
else
  read -r -s -p "Senha para '$USER_NAME': " PASSWORD
  echo
fi

cd "$(dirname "$0")"
CONFIG_DIR="$PWD/mosquitto/config"
mkdir -p "$CONFIG_DIR"

# O Mosquitto roda como uid 1883 dentro do container
docker run --rm --user root \
  -v "$CONFIG_DIR:/mosquitto/config" \
  eclipse-mosquitto:2 sh -c "
    mosquitto_passwd -c -b /mosquitto/config/passwd '$USER_NAME' '$PASSWORD' &&
    chown 1883:1883 /mosquitto/config/passwd &&
    chmod 0600 /mosquitto/config/passwd
  "

echo "Pronto. Agora rode: docker compose up -d"