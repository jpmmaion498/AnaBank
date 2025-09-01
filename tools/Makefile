# Makefile para AnaBank
.PHONY: help build test run clean docker-up docker-down

# Variáveis
SOLUTION = AnaBank.sln
ACCOUNTS_PROJECT = src/Accounts/AnaBank.Accounts.API/AnaBank.Accounts.API.csproj
TRANSFERS_PROJECT = src/Transfers/AnaBank.Transfers.API/AnaBank.Transfers.API.csproj
FEES_PROJECT = src/Fees/AnaBank.Fees.Worker/AnaBank.Fees.Worker.csproj

help: ## Mostra esta ajuda
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-15s\033[0m %s\n", $$1, $$2}'

# Build e Restore
build: ## Compila toda a solução
	dotnet build $(SOLUTION)

restore: ## Restaura dependências NuGet
	dotnet restore $(SOLUTION)

clean: ## Limpa arquivos de build
	dotnet clean $(SOLUTION)
	rm -rf bin obj test-results coverage

# Testes
test: ## Executa todos os testes
	dotnet test --logger trx --results-directory test-results

test-unit: ## Executa apenas testes unitários
	dotnet test tests/AnaBank.Accounts.UnitTests/
	dotnet test tests/AnaBank.Transfers.UnitTests/

test-integration: ## Executa apenas testes de integração
	dotnet test tests/AnaBank.Accounts.IntegrationTests/

test-coverage: ## Executa testes com cobertura
	dotnet test --collect:"XPlat Code Coverage" --results-directory coverage

# Execução local
run-accounts: ## Executa API de Accounts
	dotnet run --project $(ACCOUNTS_PROJECT)

run-transfers: ## Executa API de Transfers
	dotnet run --project $(TRANSFERS_PROJECT)

run-fees: ## Executa Worker de Fees
	dotnet run --project $(FEES_PROJECT)

# Desenvolvimento com hot-reload
dev-accounts: ## Executa Accounts em modo desenvolvimento
	cd src/Accounts/AnaBank.Accounts.API && dotnet watch run

dev-transfers: ## Executa Transfers em modo desenvolvimento
	cd src/Transfers/AnaBank.Transfers.API && dotnet watch run

# Docker
docker-build: ## Constrói imagens Docker
	cd deploy && docker-compose build

docker-up: ## Sobe todos os serviços com Docker
	cd deploy && docker-compose up -d

docker-up-dev: ## Sobe ambiente de desenvolvimento
	cd deploy && docker-compose -f docker-compose.dev.yml up -d

docker-down: ## Para todos os serviços Docker
	cd deploy && docker-compose down

docker-logs: ## Mostra logs dos containers
	cd deploy && docker-compose logs -f

docker-clean: ## Remove containers, volumes e imagens
	cd deploy && docker-compose down -v --rmi all

# Banco de dados
init-db: ## Inicializa bancos de dados
	mkdir -p deploy/data
	sqlite3 deploy/data/anabank_accounts.db < config/Scripts/accounts-sqlite.sql
	sqlite3 deploy/data/anabank_transfers.db < config/Scripts/transfers-sqlite.sql
	sqlite3 deploy/data/anabank_fees.db < config/Scripts/fees-sqlite.sql

clean-db: ## Remove bancos de dados
	rm -f deploy/data/*.db

# Environment setup
setup-dev: ## Configura ambiente de desenvolvimento
	mkdir -p deploy/data
	mkdir -p test-results
	mkdir -p coverage
	chmod 755 deploy/data

# Verificação de qualidade
check: build test ## Executa build e testes (CI/CD)

# Health checks
health: ## Verifica health dos serviços
	@echo "Checking Accounts API..."
	@curl -s http://localhost:8081/health || echo "Accounts API not running"
	@echo "Checking Transfers API..."
	@curl -s http://localhost:8082/health || echo "Transfers API not running"

# Logs específicos
logs-accounts: ## Logs da API Accounts
	cd deploy && docker-compose logs -f accounts-api

logs-transfers: ## Logs da API Transfers
	cd deploy && docker-compose logs -f transfers-api

logs-fees: ## Logs do Worker Fees
	cd deploy && docker-compose logs -f fees-worker

logs-nginx: ## Logs do Nginx
	cd deploy && docker-compose logs -f nginx

# Deploy
deploy-prod: ## Deploy para produção
	cd deploy && docker-compose -f docker-compose.yml up -d

deploy-stop: ## Para deployment de produção
	cd deploy && docker-compose -f docker-compose.yml down

# Monitoramento
ps: ## Lista containers em execução
	cd deploy && docker-compose ps

stats: ## Mostra estatísticas dos containers
	docker stats