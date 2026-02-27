#!/bin/bash

# Script para configurar git hooks de segurança

echo "🔐 Configurando Git Hooks de Segurança..."

# Copiar hooks
mkdir -p .git/hooks
cp .githooks/pre-commit .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit

echo "✅ Pre-commit hook instalado!"
echo ""
echo "Agora, antes de cada commit, o hook vai:"
echo "  1. ❌ Bloquear arquivos sensíveis (appsettings.Development.json, .env)"
echo "  2. ❌ Detectar possíveis credenciais no código"
echo "  3. ✅ Permitir commit se tudo estiver seguro"
echo ""
echo "Para ignorar o hook (NÃO RECOMENDADO):"
echo "  git commit --no-verify"
