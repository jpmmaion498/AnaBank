# AnaBank - Banco Digital

Sistema de microsservi�os para o Banco Digital da Ana, desenvolvido em .NET 8 seguindo padr�es DDD + CQRS.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)](https://www.docker.com/)
[![Tests](https://img.shields.io/badge/Tests-37%20Passing-green.svg)](#testes)
[![Architecture](https://img.shields.io/badge/Architecture-DDD%20%2B%20CQRS-orange.svg)](#arquitetura)

## 🚀 **EXECUÇÃO RÁPIDA**

### **Pré-requisito único:**
- ✅ **Docker Desktop** ([Download](https://www.docker.com/products/docker-desktop/))

### **Iniciar sistema completo:**
```bash
.\INICIAR-ANABANK.bat
```

### **Testar no Postman:**
1. **Importe**: `AnaBank-Complete.postman_collection.json`
2. **Importe**: `AnaBank-Production.postman_environment.json`
3. **Execute** a collection completa (testes autom�ticos)

**⚡ Resultado esperado:** Sistema funcionando com saldos finais calculados automaticamente

---

## Estrutura do Projeto

```
AnaBank/
├── src/                          # Código fonte
│   ├── Accounts/                 # Microsserviço de Contas
│   ├── Transfers/                # Microsserviço de Transferências  
│   ├── Fees/                     # Worker de Tarifas
│   └── BuildingBlocks/           # Componentes compartilhados
├── tests/                        # Testes automatizados
│   ├── AnaBank.Accounts.UnitTests/
│   ├── AnaBank.Transfers.UnitTests/
│   └── AnaBank.Accounts.IntegrationTests/
├── deploy/                       # Docker e deployment
│   ├── docker-compose.yml        # Produção
│   ├── docker-compose.dev.yml    # Desenvolvimento
│   └── nginx/                    # Load balancer
├── config/                       # Configurações
│   ├── Scripts/                  # Scripts SQL
│   └── *.md                      # Documentação de configuração
└── docs/                         # Documentação
    ├── ARCHITECTURE.md           # Arquitetura detalhada
    └── API_GUIDE.md              # Guia das APIs
```

## Arquitetura

### Microsservi�os
- **Accounts.API** (porta 8091): Gest�o de contas correntes, movimenta��es e saldo
- **Transfers.API** (porta 8092): Transfer�ncias entre contas
- **Fees.Worker**: Processamento de tarifas via BackgroundService

### Tecnologias
- **.NET 8** - Framework principal
- **SQLite + Dapper** - Banco de dados e ORM
- **MediatR** - Pattern CQRS
- **JWT** - Autentica��o e autoriza��o
- **FluentValidation** - Valida��es robustas
- **Docker** - Containeriza��o
- **Kafka** - Mensageria ass�ncrona
- **Nginx** - Load balancer/proxy

## Quick Start

### Op��o 1: Sistema Completo (Recomendado)

```bash
# Execute o script �nico
.\INICIAR-ANABANK.bat
```

### Op��o 2: Docker Compose

```bash
# Sistema completo
docker-compose -f docker-compose.production.yml up -d

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
dotnet run --urls="http://localhost:8091"

# Terminal 2 - Transfers API  
cd src/Transfers/AnaBank.Transfers.API
dotnet run --urls="http://localhost:8092"
```

## URLs dos Servi�os

| Servi�o | URL | Swagger |
|---------|-----|---------|
| **Accounts API** | http://localhost:8091 | http://localhost:8091/swagger |
| **Transfers API** | http://localhost:8092 | http://localhost:8092/swagger |
| **Nginx (Prod)** | http://localhost | - |

## APIs Principais

### Accounts API

| Endpoint | Método | Descrição | Auth |
|----------|--------|-----------|------|
| `/api/accounts` | POST | Cadastrar conta | ❌ |
| `/api/accounts/login` | POST | Login (gera JWT) | ❌ |
| `/api/accounts/balance` | GET | Consultar saldo | ✅ |
| `/api/accounts/movements` | POST | Movimentação (C/D) | ✅ |
| `/api/accounts/deactivate` | POST | Desativar conta | ✅ |

### Transfers API

| Endpoint | Método | Descrição | Auth |
|----------|--------|-----------|------|
| `/api/transfers` | POST | Transferência entre contas | ✅ |

> **Documenta��o completa**: [API Guide](docs/API_GUIDE.md)

## Testes

```bash
# Executar todos os testes
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

**Status atual**: ✅ **37 testes unitários** passando

## Seguran�a

- ✅ **JWT obrigatório** em todos os endpoints (exceto cadastro/login)
- ✅ **Validação de CPF** com algoritmo padrão brasileiro
- ✅ **Hash de senhas** com salt único
- ✅ **Idempotência** via header `Idempotency-Key`
- ✅ **CORS** configurado

## Exemplo de Uso

```bash
# 1. Cadastrar conta
curl -X POST http://localhost:8091/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"name":"Carlos Silva","cpf":"11144477735","password":"senha123"}'

# 2. Login (recebe JWT)
curl -X POST http://localhost:8091/api/accounts/login \
  -H "Content-Type: application/json" \
  -d '{"cpfOrNumber":"11144477735","password":"senha123"}'

# 3. Depositar
curl -X POST http://localhost:8091/api/accounts/movements \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"type":"C","value":1000.00}'

# 4. Transferir
curl -X POST http://localhost:8092/api/transfers \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"destinationAccountNumber":"87654321","value":100.00}'
```

## Monitoramento

### Health Checks
```bash
curl http://localhost:8091/health  # Accounts
curl http://localhost:8092/health  # Transfers
```

### Logs via Docker
```bash
# Ver logs do sistema completo
docker-compose -f docker-compose.production.yml logs -f

# Logs espec�ficos
docker-compose -f docker-compose.production.yml logs -f accounts-api
docker-compose -f docker-compose.production.yml logs -f transfers-api
docker-compose -f docker-compose.production.yml logs -f fees-worker
```

## Caracter�sticas T�cnicas

### Requisitos Atendidos
- **DDD + CQRS** - Arquitetura bem definida
- **JWT obrigat�rio** - Seguran�a implementada
- **Idempot�ncia** - Opera��es seguras para retry
- **Swagger completo** - Documenta��o interativa
- **Testes automatizados** - Unit�rios e integra��o
- **Docker-compose** - Deploy containerizado
- **SQLite** - Banco de dados conforme solicitado

### Diferenciais Implementados
- **Kafka** - Mensageria ass�ncrona para tarifas
- **Nginx Load Balancer** - Escalabilidade
- **BackgroundService** - Processamento ass�ncrono
- **Health Checks** - Monitoramento
- **Structured Logging** - Observabilidade

## Tipos de Erro Padronizados

| C�digo | Tipo | Descri��o |
|--------|------|-----------|
| `INVALID_DOCUMENT` | 400 | CPF inv�lido |
| `USER_UNAUTHORIZED` | 401 | Credenciais inv�lidas |
| `INVALID_ACCOUNT` | 400 | Conta n�o encontrada |
| `INACTIVE_ACCOUNT` | 400 | Conta inativa |
| `INVALID_VALUE` | 400 | Valor inv�lido |
| `INVALID_TYPE` | 400 | Tipo de movimenta��o inv�lido |
| `INSUFFICIENT_FUNDS` | 400 | Saldo insuficiente |

## Documenta��o

- [Arquitetura Detalhada](docs/ARCHITECTURE.md)
- [Guia de APIs](docs/API_GUIDE.md)
- [Deploy com Docker](deploy/)
- [Sistema AnaBank](SISTEMA-ANABANK.md)
- [Guia R�pido](GUIA-RAPIDO.md)

## Contribui��o

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit: `git commit -m 'Add nova funcionalidade'`
4. Push: `git push origin feature/nova-funcionalidade`
5. Abra um Pull Request

## Licen�a

Este projeto est� sob a licen�a MIT.

---

**AnaBank - Seu banco digital de confian�a!**