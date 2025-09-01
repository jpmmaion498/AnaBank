#!/bin/bash

# Script de parada do AnaBank
echo "?? Parando AnaBank..."

cd deploy

# Parar containers
echo "?? Parando containers..."
docker-compose down

# Verificar se pararam
echo "?? Status dos containers:"
docker-compose ps

echo "? AnaBank parado com sucesso!"