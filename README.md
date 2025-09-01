# ?? AnaBank - Banco Digital

Sistema de microsserviços para o Banco Digital da Ana, desenvolvido em .NET 8 seguindo padrões DDD + CQRS.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)](https://www.docker.com/)
[![Tests](https://img.shields.io/badge/Tests-37%20Passing-green.svg)](#testes)
[![Architecture](https://img.shields.io/badge/Architecture-DDD%20%2B%20CQRS-orange.svg)](#arquitetura)

## ?? Estrutura do Projeto

```
AnaBank/
??? ?? src/                          # Código fonte
?   ??? ?? Accounts/                 # Microsserviço de Contas
?   ??? ?? Transfers/                # Microsserviço de Transferências  
?   ??? ?? Fees/                     # Worker de Tarifas (opcional)
?   ??? ?? BuildingBlocks/           # Componentes compartilhados
??? ?? tests/                        # Testes automatizados
?   ??? ?? AnaBank.Accounts.UnitTests/
?   ??? ?? AnaBank.Transfers.UnitTests/
?   ??? ?? AnaBank.Accounts.IntegrationTests/
??? ?? deploy/                       # Docker e deployment
?   ??? ?? docker-compose.yml       # Produção
?   ??? ?? docker-compose.dev.yml   # Desenvolvimento
?   ??? ?? nginx/                    # Load balancer
??? ?? config/                       # Configurações
?   ??? ?? appsettings.*.json       # Configurações por ambiente
?   ??? ?? Scripts/                  # Scripts SQL
??? ?? tools/                        # Ferramentas e scripts
?   ??? ?? Makefile                  # Comandos de automação
?   ??? ?? start.sh/start.bat        # Scripts de inicialização
?   ??? ?? stop.sh                   # Scripts de parada
??? ?? docs/                         # Documentação
    ??? ?? ARCHITECTURE.md           # Arquitetura detalhada
    ??? ?? API_GUIDE.md             # Guia das APIs
```

## ??? Arquitetura

### Microsserviços
- **?? Accounts.API** (8081): Gestão de contas correntes, movimentações e saldo
- **?? Transfers.API** (8082): Transferências entre contas
- **?? Fees.Worker**: Processamento de tarifas via BackgroundService

### Tecnologias
- **.NET 8** - Framework principal
- **SQLite + Dapper** - Banco de dados e ORM
- **MediatR** - Pattern CQRS
- **JWT** - Autenticação e autorização
- **FluentValidation** - Validações robustas
- **Docker** - Containerização
- **Nginx** - Load balancer/proxy
- **Redis** - Cache (diferencial)

## ?? Quick Start

### Opção 1: Scripts Automatizados (Recomendado)

```bash
# Linux/Mac
./tools/start.sh

# Windows
tools\start.bat
```

### Opção 2: Docker Compose

```bash
# Desenvolvimento
cd deploy
docker-compose -f docker-compose.dev.yml up -d

# Produção
cd deploy
docker-compose up -d
```

### Opção 3: Execução Local

```bash
# Terminal 1 - Accounts API
cd src/Accounts/AnaBank.Accounts.API
dotnet run

# Terminal 2 - Transfers API  
cd src/Transfers/AnaBank.Transfers.API
dotnet run
```

## ?? URLs dos Serviços

| Serviço | URL | Swagger |
|---------|-----|---------|
| **Accounts API** | http://localhost:8081 | http://localhost:8081 |
| **Transfers API** | http://localhost:8082 | http://localhost:8082 |
| **Nginx (Prod)** | http://localhost | - |
| **Redis** | localhost:6379 | - |

## ?? APIs Principais

### ?? Accounts API

| Endpoint | Método | Descrição | Auth |
|----------|--------|-----------|------|
| `/api/accounts` | POST | Cadastrar conta | ? |
| `/api/accounts/login` | POST | Login (gera JWT) | ? |
| `/api/accounts/balance` | GET | Consultar saldo | ? |
| `/api/accounts/movements` | POST | Movimentação (C/D) | ? |
| `/api/accounts/deactivate` | POST | Desativar conta | ? |

### ?? Transfers API

| Endpoint | Método | Descrição | Auth |
|----------|--------|-----------|------|
| `/api/transfers` | POST | Transferência entre contas | ? |

> ?? **Documentação completa**: [API Guide](docs/API_GUIDE.md)

## ?? Testes

```bash
# Todos os testes unitários
make test-unit

# Testes de integração
make test-integration

# Todos os testes
make test

# Cobertura de código
make test-coverage
```

**Status atual**: ? **37 testes unitários** passando

## ?? Segurança

- ? **JWT obrigatório** em todos os endpoints (exceto cadastro/login)
- ? **Validação de CPF** com algoritmo padrão brasileiro
- ? **Hash de senhas** com salt único
- ? **Idempotência** via header `Idempotency-Key`
- ? **Rate limiting** implementado
- ? **CORS** configurado

## ?? Exemplo de Uso

```bash
# 1. Cadastrar conta
curl -X POST http://localhost:8081/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"name":"Ana Silva","cpf":"12345678909","password":"123456"}'

# 2. Login (recebe JWT)
curl -X POST http://localhost:8081/api/accounts/login \
  -H "Content-Type: application/json" \
  -d '{"cpfOrNumber":"12345678909","password":"123456"}'

# 3. Depositar
curl -X POST http://localhost:8081/api/accounts/movements \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"type":"C","value":1000.00}'

# 4. Transferir
curl -X POST http://localhost:8082/api/transfers \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"destinationAccountNumber":"87654321","value":100.00}'
```

## ??? Comandos Úteis

```bash
# Makefile helpers (dentro de tools/)
make help           # Ver todos os comandos
make build          # Compilar solução
make test           # Executar testes
make docker-up      # Subir containers
make docker-down    # Parar containers
make health         # Verificar saúde dos serviços
make logs-accounts  # Logs da API Accounts
make clean          # Limpar builds
```

## ?? Monitoramento

### Health Checks
```bash
curl http://localhost:8081/health  # Accounts
curl http://localhost:8082/health  # Transfers
```

### Logs
```bash
# Via Docker
make logs-accounts
make logs-transfers

# Status dos containers
make ps
```

## ?? Características Técnicas

### ? Requisitos Atendidos
- **DDD + CQRS** - Arquitetura bem definida
- **JWT obrigatório** - Segurança implementada
- **Idempotência** - Operações seguras para retry
- **Swagger completo** - Documentação interativa
- **Testes automatizados** - Unitários e integração
- **Docker-compose** - Deploy containerizado
- **SQLite** - Banco de dados conforme solicitado

### ?? Diferenciais Implementados
- **Redis Cache** - Performance otimizada
- **Nginx Load Balancer** - Escalabilidade
- **BackgroundService** - Processamento assíncrono
- **Health Checks** - Monitoramento
- **Rate Limiting** - Proteção contra abuso
- **Structured Logging** - Observabilidade

## ?? Tipos de Erro Padronizados

| Código | Tipo | Descrição |
|--------|------|-----------|
| `INVALID_DOCUMENT` | 400 | CPF inválido |
| `USER_UNAUTHORIZED` | 401 | Credenciais inválidas |
| `INVALID_ACCOUNT` | 400 | Conta não encontrada |
| `INACTIVE_ACCOUNT` | 400 | Conta inativa |
| `INVALID_VALUE` | 400 | Valor inválido |
| `INVALID_TYPE` | 400 | Tipo de movimentação inválido |
| `INSUFFICIENT_FUNDS` | 400 | Saldo insuficiente |

## ?? Documentação

- ?? [Arquitetura Detalhada](docs/ARCHITECTURE.md)
- ?? [Guia de APIs](docs/API_GUIDE.md)
- ?? [Deploy com Docker](deploy/)
- ?? [Scripts e Ferramentas](tools/)

## ?? Contribuição

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit: `git commit -m 'Add nova funcionalidade'`
4. Push: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

## ?? Licença

Este projeto está sob a licença MIT.

---

**?? AnaBank - Seu banco digital de confiança!**