#!/bin/bash

# Script de inicializa��o do AnaBank
echo "?? Inicializando AnaBank..."

# Verificar se Docker est� rodando
if ! docker info > /dev/null 2>&1; then
    echo "? Docker n�o est� rodando. Por favor, inicie o Docker primeiro."
    exit 1
fi

# Criar diret�rios necess�rios
echo "?? Criando diret�rios..."
mkdir -p deploy/data
mkdir -p test-results
mkdir -p coverage

# Verificar se os bancos existem, sen�o criar
if [ ! -f "deploy/data/anabank_accounts.db" ]; then
    echo "??? Criando banco de dados Accounts..."
    sqlite3 deploy/data/anabank_accounts.db < config/Scripts/accounts-sqlite.sql
fi

if [ ! -f "deploy/data/anabank_transfers.db" ]; then
    echo "??? Criando banco de dados Transfers..."
    sqlite3 deploy/data/anabank_transfers.db < config/Scripts/transfers-sqlite.sql
fi

if [ ! -f "deploy/data/anabank_fees.db" ]; then
    echo "??? Criando banco de dados Fees..."
    sqlite3 deploy/data/anabank_fees.db < config/Scripts/fees-sqlite.sql
fi

# Compilar a solu��o
echo "?? Compilando solu��o..."
dotnet build

if [ $? -ne 0 ]; then
    echo "? Falha na compila��o. Abortando."
    exit 1
fi

# Executar testes
echo "?? Executando testes..."
dotnet test --filter "UnitTests"

if [ $? -ne 0 ]; then
    echo "?? Alguns testes falharam, mas continuando..."
fi

# Subir containers
echo "?? Subindo containers Docker..."
cd deploy
docker-compose up -d

# Aguardar servi�os ficarem prontos
echo "? Aguardando servi�os ficarem prontos..."
sleep 10

# Verificar health
echo "?? Verificando sa�de dos servi�os..."
curl -s http://localhost:8081/health > /dev/null
if [ $? -eq 0 ]; then
    echo "? Accounts API est� rodando"
else
    echo "? Accounts API n�o respondeu"
fi

curl -s http://localhost:8082/health > /dev/null
if [ $? -eq 0 ]; then
    echo "? Transfers API est� rodando"
else
    echo "? Transfers API n�o respondeu"
fi

echo ""
echo "?? AnaBank inicializado com sucesso!"
echo ""
echo "?? URLs dispon�veis:"
echo "   � Accounts API: http://localhost:8081"
echo "   � Transfers API: http://localhost:8082"
echo "   � Swagger Accounts: http://localhost:8081"
echo "   � Swagger Transfers: http://localhost:8082"
echo ""
echo "?? Comandos �teis:"
echo "   � Ver logs: make logs"
echo "   � Parar tudo: make docker-down"
echo "   � Status: make ps"
echo ""