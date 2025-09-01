# AnaBank - Estrutura do Projeto (Versão ASCII Simples)

## Alternativa para GitHub (caso ainda haja problemas com caracteres)

```
AnaBank/
|
+-- src/                          (Código fonte)
|   |
|   +-- Accounts/                 (Microsserviço de Contas)
|   |   +-- AnaBank.Accounts.API/
|   |   +-- AnaBank.Accounts.Application/
|   |   +-- AnaBank.Accounts.Domain/
|   |   +-- AnaBank.Accounts.Infrastructure/
|   |
|   +-- Transfers/                (Microsserviço de Transferências)
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
|   +-- docker-compose.yml       (Produção)
|   +-- docker-compose.dev.yml   (Desenvolvimento)
|   +-- nginx/
|       +-- nginx.conf            (Load balancer)
|
+-- config/                       (Configurações)
|   +-- Scripts/                  (Scripts SQL)
|   |   +-- accounts-sqlite.sql
|   |   +-- transfers-sqlite.sql
|   |   +-- fees-sqlite.sql
|   |
|   +-- README.md                 (Documentação)
|   +-- database-setup.md
|   +-- environment-variables.md
|
+-- tools/                        (Ferramentas e scripts)
|   +-- Makefile                  (Comandos de automação)
|   +-- start.sh                  (Linux/Mac)
|   +-- start.bat                 (Windows)
|   +-- stop.sh                   (Scripts de parada)
|
+-- docs/                         (Documentação)
|   +-- ARCHITECTURE.md           (Arquitetura detalhada)
|   +-- API_GUIDE.md              (Guia das APIs)
|
+-- README.md                     (Este arquivo)
+-- .gitignore
+-- AnaBank.sln                   (Solução principal)
```

## Componentes Principais

### APIs (Microsserviços)
- **Accounts.API** - Porta 8081
  - Cadastro de contas
  - Login e autenticação
  - Movimentações (crédito/débito)
  - Consulta de saldo
  - Desativação de contas

- **Transfers.API** - Porta 8082
  - Transferências entre contas
  - Validações de negócio
  - Estorno automático em falhas

### Worker Services
- **Fees.Worker** - BackgroundService
  - Processamento de tarifas
  - Consumo de eventos (Kafka)
  - Cálculo automático de taxas

### BuildingBlocks (Componentes Compartilhados)
- **Web** - JWT, Middleware, ProblemDetails
- **Data** - Connection Factory, Repositórios base

### Infraestrutura
- **Docker Compose** - Orquestração de containers
- **Nginx** - Load balancer e proxy reverso
- **Redis** - Cache distribuído
- **SQLite** - Banco de dados por serviço

### Scripts e Ferramentas
- **Makefile** - Comandos de automação
- **Scripts de inicialização** - start.sh/start.bat
- **Scripts SQL** - Criação de schemas
- **Docker** - Containerização completa