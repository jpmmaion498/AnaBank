# AnaBank - Estrutura do Projeto (Vers�o ASCII Simples)

## Alternativa para GitHub (caso ainda haja problemas com caracteres)

```
AnaBank/
|
+-- src/                          (C�digo fonte)
|   |
|   +-- Accounts/                 (Microsservi�o de Contas)
|   |   +-- AnaBank.Accounts.API/
|   |   +-- AnaBank.Accounts.Application/
|   |   +-- AnaBank.Accounts.Domain/
|   |   +-- AnaBank.Accounts.Infrastructure/
|   |
|   +-- Transfers/                (Microsservi�o de Transfer�ncias)
|   |   +-- AnaBank.Transfers.API/
|   |   +-- AnaBank.Transfers.Application/
|   |   +-- AnaBank.Transfers.Domain/
|   |   +-- AnaBank.Transfers.Infrastructure/
|   |
|   +-- Fees/                     (Worker de Tarifas)
|   |   +-- AnaBank.Fees.Worker/
|   |
|   +-- BuildingBlocks/           (Componentes compartilhados)
|       +-- AnaBank.BuildingBlocks.Web/
|       +-- AnaBank.BuildingBlocks.Data/
|
+-- tests/                        (Testes automatizados)
|   +-- AnaBank.Accounts.UnitTests/
|   +-- AnaBank.Transfers.UnitTests/
|   +-- AnaBank.Accounts.IntegrationTests/
|
+-- deploy/                       (Docker e deployment)
|   +-- docker-compose.yml       (Produ��o)
|   +-- docker-compose.dev.yml   (Desenvolvimento)
|   +-- nginx/
|       +-- nginx.conf            (Load balancer)
|
+-- config/                       (Configura��es)
|   +-- Scripts/                  (Scripts SQL)
|   |   +-- accounts-sqlite.sql
|   |   +-- transfers-sqlite.sql
|   |   +-- fees-sqlite.sql
|   |
|   +-- README.md                 (Documenta��o)
|   +-- database-setup.md
|   +-- environment-variables.md
|
+-- tools/                        (Ferramentas e scripts)
|   +-- Makefile                  (Comandos de automa��o)
|   +-- start.sh                  (Linux/Mac)
|   +-- start.bat                 (Windows)
|   +-- stop.sh                   (Scripts de parada)
|
+-- docs/                         (Documenta��o)
|   +-- ARCHITECTURE.md           (Arquitetura detalhada)
|   +-- API_GUIDE.md              (Guia das APIs)
|
+-- README.md                     (Este arquivo)
+-- .gitignore
+-- AnaBank.sln                   (Solu��o principal)
```

## Componentes Principais

### APIs (Microsservi�os)
- **Accounts.API** - Porta 8081
  - Cadastro de contas
  - Login e autentica��o
  - Movimenta��es (cr�dito/d�bito)
  - Consulta de saldo
  - Desativa��o de contas

- **Transfers.API** - Porta 8082
  - Transfer�ncias entre contas
  - Valida��es de neg�cio
  - Estorno autom�tico em falhas

### Worker Services
- **Fees.Worker** - BackgroundService
  - Processamento de tarifas
  - Consumo de eventos (Kafka)
  - C�lculo autom�tico de taxas

### BuildingBlocks (Componentes Compartilhados)
- **Web** - JWT, Middleware, ProblemDetails
- **Data** - Connection Factory, Reposit�rios base

### Infraestrutura
- **Docker Compose** - Orquestra��o de containers
- **Nginx** - Load balancer e proxy reverso
- **Redis** - Cache distribu�do
- **SQLite** - Banco de dados por servi�o

### Scripts e Ferramentas
- **Makefile** - Comandos de automa��o
- **Scripts de inicializa��o** - start.sh/start.bat
- **Scripts SQL** - Cria��o de schemas
- **Docker** - Containeriza��o completa