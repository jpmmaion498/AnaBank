# Makefile para AnaBank
.PHONY: help build test run clean docker-up docker-down

# Variáveis
SOLUTION = AnaBank.sln
ACCOUNTS_PROJECT = src/Accounts/AnaBank.Accounts.API/AnaBank.Accounts.API.csproj
TRANSFERS_PROJECT = src/Transfers/AnaBank.Transfers.API/AnaBank.Transfers.API.csproj
FEES_PROJECT = src/Fees/AnaBank.Fees.Worker/AnaBank.Fees.Worker.csproj

help: ## Mostra esta ajuda
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-15s\033[0m %s\n", $$1, $$2}'

build: ## Compila toda a solução
	dotnet build $(SOLUTION)

restore: ## Restaura dependências NuGet
	dotnet restore $(SOLUTION)

test: ## Executa todos os testes
	dotnet test --logger trx --results-directory test-results

test-unit: ## Executa apenas testes unitários
	dotnet test tests/AnaBank.Accounts.UnitTests/
	dotnet test tests/AnaBank.Transfers.UnitTests/

test-integration: ## Executa apenas testes de integração
	dotnet test tests/AnaBank.Accounts.IntegrationTests/

run-accounts: ## Executa API de Accounts
	dotnet run --project $(ACCOUNTS_PROJECT)

run-transfers: ## Executa API de Transfers
	dotnet run --project $(TRANSFERS_PROJECT)

run-fees: ## Executa Worker de Fees
	dotnet run --project $(FEES_PROJECT)

clean: ## Limpa arquivos de build
	dotnet clean $(SOLUTION)
	rm -rf bin obj test-results

docker-build: ## Constrói imagens Docker
	docker-compose build

docker-up: ## Sobe todos os serviços com Docker
	docker-compose up -d

docker-down: ## Para todos os serviços Docker
	docker-compose down

docker-logs: ## Mostra logs dos containers
	docker-compose logs -f

setup-dev: ## Configura ambiente de desenvolvimento
	mkdir -p data
	chmod 755 data

# Comandos de desenvolvimento
dev-accounts: ## Executa Accounts em modo desenvolvimento
	cd src/Accounts/AnaBank.Accounts.API && dotnet watch run

dev-transfers: ## Executa Transfers em modo desenvolvimento
	cd src/Transfers/AnaBank.Transfers.API && dotnet watch run

# Scripts de banco
init-db: ## Inicializa bancos de dados
	sqlite3 data/anabank_accounts.db < Scripts/accounts-sqlite.sql
	sqlite3 data/anabank_transfers.db < Scripts/transfers-sqlite.sql
	sqlite3 data/anabank_fees.db < Scripts/fees-sqlite.sql

# Limpeza de bancos
clean-db: ## Remove bancos de dados
	rm -f data/*.db

# Coverage de testes
test-coverage: ## Executa testes com cobertura
	dotnet test --collect:"XPlat Code Coverage" --results-directory coverage

# Verificação de qualidade
check: build test ## Executa build e testes (CI/CD)

# Docker Compose helpers
logs-accounts: ## Logs da API Accounts
	docker-compose logs -f accounts-api

logs-transfers: ## Logs da API Transfers
	docker-compose logs -f transfers-api

logs-fees: ## Logs do Worker Fees
	docker-compose logs -f fees-worker

# Health checks
health: ## Verifica health dos serviços
	@echo "Checking Accounts API..."
	@curl -s http://localhost:8081/health || echo "Accounts API not running"
	@echo "Checking Transfers API..."
	@curl -s http://localhost:8082/health || echo "Transfers API not running"