#!/bin/bash
# FoodTrack ERP — Codex Runner Startup Script
# ==============================================
# Spusti toto na svojom ThinkPade v tmux:

# 1. Najprv nainštaluj .NET 8 SDK (ak ešte nemáš):
# wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
# chmod +x dotnet-install.sh
# ./dotnet-install.sh --channel 8.0
# export PATH="$HOME/.dotnet:$PATH"

# 2. Nainštaluj MAUI workload:
# dotnet workload install maui-android

# 3. Inicializuj git repo:
cd ~/foodtrack-erp
git init
git add .plan/ AGENTS.md
git commit -m "chore: init project scaffolding with codex-runner plan"

# 4. Spusti codex-runner:
codex-runner run ~/foodtrack-erp \
  --task "Build FoodTrack ERP — a complete food industry ERP demo. Read AGENTS.md first, then follow .plan/FINISH_CRITERIA.md and .plan/TODO.md. Start with the solution structure and domain entities, then build up layer by layer. Use Slovak product names in seed data. Make it production-quality." \
  --idle-seconds 10 \
  --shower-interval 8

# 5. Ráno skontroluj výsledok:
# codex-runner status ~/foodtrack-erp
# cd ~/foodtrack-erp && dotnet build
# cd src/FoodTrack.API && dotnet run
# Otvor http://localhost:5000/swagger
