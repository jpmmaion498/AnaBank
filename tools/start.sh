#!/bin/bash

# Script de inicialização do AnaBank
echo "?? Inicializando AnaBank..."

# Verificar se Docker está rodando
if ! docker info > /dev/null 2>&1; then
    echo "? Docker não está rodando. Por favor, inicie o Docker primeiro."
    exit 1
fi

# Criar diretórios necessários
echo "?? Criando diretórios..."
mkdir -p deploy/data
mkdir -p test-results
mkdir -p coverage

# Verificar se os bancos existem, senão criar
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

# Compilar a solução
echo "?? Compilando solução..."
dotnet build

if [ $? -ne 0 ]; then
    echo "? Falha na compilação. Abortando."
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

# Aguardar serviços ficarem prontos
echo "? Aguardando serviços ficarem prontos..."
sleep 10

# Verificar health
echo "?? Verificando saúde dos serviços..."
curl -s http://localhost:8081/health > /dev/null
if [ $? -eq 0 ]; then
    echo "? Accounts API está rodando"
else
    echo "? Accounts API não respondeu"
fi

curl -s http://localhost:8082/health > /dev/null
if [ $? -eq 0 ]; then
    echo "? Transfers API está rodando"
else
    echo "? Transfers API não respondeu"
fi

echo ""
echo "?? AnaBank inicializado com sucesso!"
echo ""
echo "?? URLs disponíveis:"
echo "   • Accounts API: http://localhost:8081"
echo "   • Transfers API: http://localhost:8082"
echo "   • Swagger Accounts: http://localhost:8081"
echo "   • Swagger Transfers: http://localhost:8082"
echo ""
echo "?? Comandos úteis:"
echo "   • Ver logs: make logs"
echo "   • Parar tudo: make docker-down"
echo "   • Status: make ps"
echo ""