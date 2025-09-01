# ?? AnaBank - Banco Digital

Sistema de microsservi�os para o Banco Digital da Ana, desenvolvido em .NET 8 seguindo padr�es DDD + CQRS.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)](https://www.docker.com/)
[![Tests](https://img.shields.io/badge/Tests-37%20Passing-green.svg)](#testes)
[![Architecture](https://img.shields.io/badge/Architecture-DDD%20%2B%20CQRS-orange.svg)](#arquitetura)

## ?? Estrutura do Projeto

```
AnaBank/
??? ?? src/                          # C�digo fonte
?   ??? ?? Accounts/                 # Microsservi�o de Contas
?   ??? ?? Transfers/                # Microsservi�o de Transfer�ncias  
?   ??? ?? Fees/                     # Worker de Tarifas (opcional)
?   ??? ?? BuildingBlocks/           # Componentes compartilhados
??? ?? tests/                        # Testes automatizados
?   ??? ?? AnaBank.Accounts.UnitTests/
?   ??? ?? AnaBank.Transfers.UnitTests/
?   ??? ?? AnaBank.Accounts.IntegrationTests/
??? ?? deploy/                       # Docker e deployment
?   ??? ?? docker-compose.yml       # Produ��o
?   ??? ?? docker-compose.dev.yml   # Desenvolvimento
?   ??? ?? nginx/                    # Load balancer
??? ?? config/                       # Configura��es
?   ??? ?? appsettings.*.json       # Configura��es por ambiente
?   ??? ?? Scripts/                  # Scripts SQL
??? ?? tools/                        # Ferramentas e scripts
?   ??? ?? Makefile                  # Comandos de automa��o
?   ??? ?? start.sh/start.bat        # Scripts de inicializa��o
?   ??? ?? stop.sh                   # Scripts de parada
??? ?? docs/                         # Documenta��o
    ??? ?? ARCHITECTURE.md           # Arquitetura detalhada
    ??? ?? API_GUIDE.md             # Guia das APIs
```

## ??? Arquitetura

### Microsservi�os
- **?? Accounts.API** (8081): Gest�o de contas correntes, movimenta��es e saldo
- **?? Transfers.API** (8082): Transfer�ncias entre contas
- **?? Fees.Worker**: Processamento de tarifas via BackgroundService

### Tecnologias
- **.NET 8** - Framework principal
- **SQLite + Dapper** - Banco de dados e ORM
- **MediatR** - Pattern CQRS
- **JWT** - Autentica��o e autoriza��o
- **FluentValidation** - Valida��es robustas
- **Docker** - Containeriza��o
- **Nginx** - Load balancer/proxy
- **Redis** - Cache (diferencial)

## ?? Quick Start

### Op��o 1: Scripts Automatizados (Recomendado)

```bash
# Linux/Mac
./tools/start.sh

# Windows
tools\start.bat
```

### Op��o 2: Docker Compose

```bash
# Desenvolvimento
cd deploy
docker-compose -f docker-compose.dev.yml up -d

# Produ��o
cd deploy
docker-compose up -d
```

### Op��o 3: Execu��o Local

```bash
# Terminal 1 - Accounts API
cd src/Accounts/AnaBank.Accounts.API
dotnet run

# Terminal 2 - Transfers API  
cd src/Transfers/AnaBank.Transfers.API
dotnet run
```

## ?? URLs dos Servi�os

| Servi�o | URL | Swagger |
|---------|-----|---------|
| **Accounts API** | http://localhost:8081 | http://localhost:8081 |
| **Transfers API** | http://localhost:8082 | http://localhost:8082 |
| **Nginx (Prod)** | http://localhost | - |
| **Redis** | localhost:6379 | - |

## ?? APIs Principais

### ?? Accounts API

| Endpoint | M�todo | Descri��o | Auth |
|----------|--------|-----------|------|
| `/api/accounts` | POST | Cadastrar conta | ? |
| `/api/accounts/login` | POST | Login (gera JWT) | ? |
| `/api/accounts/balance` | GET | Consultar saldo | ? |
| `/api/accounts/movements` | POST | Movimenta��o (C/D) | ? |
| `/api/accounts/deactivate` | POST | Desativar conta | ? |

### ?? Transfers API

| Endpoint | M�todo | Descri��o | Auth |
|----------|--------|-----------|------|
| `/api/transfers` | POST | Transfer�ncia entre contas | ? |

> ?? **Documenta��o completa**: [API Guide](docs/API_GUIDE.md)

## ?? Testes

```bash
# Todos os testes unit�rios
make test-unit

# Testes de integra��o
make test-integration

# Todos os testes
make test

# Cobertura de c�digo
make test-coverage
```

**Status atual**: ? **37 testes unit�rios** passando

## ?? Seguran�a

- ? **JWT obrigat�rio** em todos os endpoints (exceto cadastro/login)
- ? **Valida��o de CPF** com algoritmo padr�o brasileiro
- ? **Hash de senhas** com salt �nico
- ? **Idempot�ncia** via header `Idempotency-Key`
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

## ??? Comandos �teis

```bash
# Makefile helpers (dentro de tools/)
make help           # Ver todos os comandos
make build          # Compilar solu��o
make test           # Executar testes
make docker-up      # Subir containers
make docker-down    # Parar containers
make health         # Verificar sa�de dos servi�os
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

## ?? Caracter�sticas T�cnicas

### ? Requisitos Atendidos
- **DDD + CQRS** - Arquitetura bem definida
- **JWT obrigat�rio** - Seguran�a implementada
- **Idempot�ncia** - Opera��es seguras para retry
- **Swagger completo** - Documenta��o interativa
- **Testes automatizados** - Unit�rios e integra��o
- **Docker-compose** - Deploy containerizado
- **SQLite** - Banco de dados conforme solicitado

### ?? Diferenciais Implementados
- **Redis Cache** - Performance otimizada
- **Nginx Load Balancer** - Escalabilidade
- **BackgroundService** - Processamento ass�ncrono
- **Health Checks** - Monitoramento
- **Rate Limiting** - Prote��o contra abuso
- **Structured Logging** - Observabilidade

## ?? Tipos de Erro Padronizados

| C�digo | Tipo | Descri��o |
|--------|------|-----------|
| `INVALID_DOCUMENT` | 400 | CPF inv�lido |
| `USER_UNAUTHORIZED` | 401 | Credenciais inv�lidas |
| `INVALID_ACCOUNT` | 400 | Conta n�o encontrada |
| `INACTIVE_ACCOUNT` | 400 | Conta inativa |
| `INVALID_VALUE` | 400 | Valor inv�lido |
| `INVALID_TYPE` | 400 | Tipo de movimenta��o inv�lido |
| `INSUFFICIENT_FUNDS` | 400 | Saldo insuficiente |

## ?? Documenta��o

- ?? [Arquitetura Detalhada](docs/ARCHITECTURE.md)
- ?? [Guia de APIs](docs/API_GUIDE.md)
- ?? [Deploy com Docker](deploy/)
- ?? [Scripts e Ferramentas](tools/)

## ?? Contribui��o

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit: `git commit -m 'Add nova funcionalidade'`
4. Push: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

## ?? Licen�a

Este projeto est� sob a licen�a MIT.

---

**?? AnaBank - Seu banco digital de confian�a!**