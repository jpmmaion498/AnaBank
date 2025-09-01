# AnaBank - Banco Digital

Sistema de microsservi�os para o Banco Digital da Ana, desenvolvido em .NET 8 seguindo padr�es DDD + CQRS.

## ??? Arquitetura

### Microsservi�os
- **Accounts.API** (8081): Gest�o de contas correntes, movimenta��es e saldo
- **Transfers.API** (8082): Transfer�ncias entre contas
- **Fees.Worker** (opcional): Processamento de tarifas via Kafka

### Tecnologias
- **.NET 8** - Framework principal
- **SQLite** - Banco de dados
- **Dapper** - ORM para acesso a dados
- **MediatR** - Pattern CQRS
- **JWT** - Autentica��o e autoriza��o
- **KafkaFlow** - Processamento ass�ncrono (opcional)
- **Redis** - Cache (diferencial)
- **Docker** - Containeriza��o
- **xUnit + FluentAssertions** - Testes

## ?? Como Executar

### Pr�-requisitos
- Docker e Docker Compose
- .NET 8 SDK (para desenvolvimento)

### Executar com Docker
```bash
# Clonar o reposit�rio
git clone <repository-url>
cd AnaBank

# Subir todos os servi�os
docker-compose up -d

# Verificar logs
docker-compose logs -f
```

### URLs dos Servi�os
- **Accounts API**: http://localhost:8081
- **Transfers API**: http://localhost:8082
- **Swagger Accounts**: http://localhost:8081
- **Swagger Transfers**: http://localhost:8082

## ?? APIs

### Accounts API

#### 1. Cadastrar Conta
```http
POST /api/accounts
Content-Type: application/json

{
  "name": "Jo�o Silva",
  "cpf": "12345678909",
  "password": "123456"
}
```

**Respostas:**
- `201`: Conta criada
- `400`: CPF inv�lido (`INVALID_DOCUMENT`)

#### 2. Login
```http
POST /api/accounts/login
Content-Type: application/json

{
  "cpfOrNumber": "12345678909",
  "password": "123456"
}
```

**Respostas:**
- `200`: Login realizado (retorna JWT)
- `401`: Credenciais inv�lidas (`USER_UNAUTHORIZED`)

#### 3. Consultar Saldo
```http
GET /api/accounts/balance
Authorization: Bearer {token}
```

**Respostas:**
- `200`: Saldo consultado
- `400`: Conta inv�lida/inativa
- `403`: Token inv�lido

#### 4. Movimenta��o
```http
POST /api/accounts/movements
Authorization: Bearer {token}
Content-Type: application/json

{
  "accountNumber": "12345678", // opcional
  "type": "C", // C = Cr�dito, D = D�bito
  "value": 100.50
}
```

**Respostas:**
- `204`: Movimenta��o realizada
- `400`: Dados inv�lidos (v�rios tipos de erro)
- `403`: Token inv�lido

#### 5. Desativar Conta
```http
POST /api/accounts/deactivate
Authorization: Bearer {token}
Content-Type: application/json

{
  "password": "123456"
}
```

### Transfers API

#### 1. Realizar Transfer�ncia
```http
POST /api/transfers
Authorization: Bearer {token}
Content-Type: application/json

{
  "destinationAccountNumber": "87654321",
  "value": 100.50
}
```

**Respostas:**
- `204`: Transfer�ncia realizada
- `400`: Dados inv�lidos/saldo insuficiente
- `403`: Token inv�lido

## ?? Seguran�a

- **JWT obrigat�rio** em todos os endpoints (exceto cadastro e login)
- **Valida��o de CPF** com algoritmo padr�o
- **Hash de senhas** com salt �nico
- **Tokens expiram** em 24 horas (configur�vel)

## ?? Idempot�ncia

O sistema suporta idempot�ncia atrav�s do header `Idempotency-Key`:

```http
POST /api/accounts/movements
Authorization: Bearer {token}
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json

{
  "type": "D",
  "value": 100.00
}
```

## ?? Testes

### Executar Testes
```bash
# Testes unit�rios
dotnet test tests/AnaBank.Accounts.UnitTests/
dotnet test tests/AnaBank.Transfers.UnitTests/

# Testes de integra��o
dotnet test tests/AnaBank.Accounts.IntegrationTests/
dotnet test tests/AnaBank.Transfers.IntegrationTests/

# Todos os testes
dotnet test
```

### Cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## ?? Kafka (Opcional)

O sistema inclui processamento ass�ncrono de tarifas:

1. **Transfer�ncia realizada** ? publica evento `transfer-completed`
2. **Fees Worker** processa ? grava tarifa no banco
3. **Worker** publica evento `fee-charged`
4. **Accounts API** consome ? debita tarifa da conta

### T�picos Kafka
- `transfer-completed`: Transfer�ncias realizadas
- `fee-charged`: Tarifas processadas

## ??? Banco de Dados

### Estrutura

**Accounts (SQLite)**
- `contacorrente`: Dados das contas
- `movimento`: Movimenta��es financeiras
- `idempotencia`: Controle de idempot�ncia

**Transfers (SQLite)**
- `transferencia`: Hist�rico de transfer�ncias
- `idempotencia`: Controle de idempot�ncia

**Fees (SQLite)**
- `tarifa`: Tarifas processadas

## ?? Configura��o

### Vari�veis de Ambiente

```bash
# JWT
JWT__SECRETKEY=sua-chave-secreta-super-segura-com-pelo-menos-32-caracteres
JWT__ISSUER=AnaBank
JWT__AUDIENCE=AnaBank.APIs
JWT__EXPIRATIONHOURS=24

# Banco
CONNECTIONSTRINGS__DEFAULTCONNECTION=Data Source=anabank.db

# Kafka (opcional)
CONNECTIONSTRINGS__KAFKA=localhost:9092

# Cache Redis
CONNECTIONSTRINGS__REDIS=localhost:6379

# Tarifas
FEESETTINGS__TRANSFERFEEAMOUNT=2.00
```

## ?? Tipos de Erro

O sistema retorna erros padronizados via ProblemDetails:

- `INVALID_DOCUMENT`: CPF inv�lido
- `USER_UNAUTHORIZED`: Credenciais inv�lidas
- `INVALID_ACCOUNT`: Conta n�o encontrada
- `INACTIVE_ACCOUNT`: Conta inativa
- `INVALID_VALUE`: Valor inv�lido
- `INVALID_TYPE`: Tipo de movimenta��o inv�lido
- `INSUFFICIENT_FUNDS`: Saldo insuficiente

## ?? Monitoramento

### Logs
```bash
# Ver logs em tempo real
docker-compose logs -f accounts-api
docker-compose logs -f transfers-api
docker-compose logs -f fees-worker
```

### Health Checks
- Accounts: http://localhost:8081/health
- Transfers: http://localhost:8082/health

## ?? Desenvolvimento

### Estrutura do Projeto
```
src/
??? Accounts/
?   ??? AnaBank.Accounts.API/
?   ??? AnaBank.Accounts.Application/
?   ??? AnaBank.Accounts.Domain/
?   ??? AnaBank.Accounts.Infrastructure/
??? Transfers/
?   ??? AnaBank.Transfers.API/
?   ??? AnaBank.Transfers.Application/
?   ??? AnaBank.Transfers.Domain/
?   ??? AnaBank.Transfers.Infrastructure/
??? Fees/
?   ??? AnaBank.Fees.Worker/
??? BuildingBlocks/
    ??? AnaBank.BuildingBlocks.Web/
    ??? AnaBank.BuildingBlocks.Data/

tests/
??? AnaBank.Accounts.UnitTests/
??? AnaBank.Accounts.IntegrationTests/
??? AnaBank.Transfers.UnitTests/
??? AnaBank.Transfers.IntegrationTests/
```

### Comandos �teis
```bash
# Restore de depend�ncias
dotnet restore

# Build da solu��o
dotnet build

# Executar localmente
dotnet run --project src/Accounts/AnaBank.Accounts.API
dotnet run --project src/Transfers/AnaBank.Transfers.API

# Executar Worker
dotnet run --project src/Fees/AnaBank.Fees.Worker
```

## ?? Exemplos de Uso

### Fluxo Completo
```bash
# 1. Criar conta
curl -X POST http://localhost:8081/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"name":"Ana Silva","cpf":"12345678909","password":"123456"}'

# 2. Fazer login
curl -X POST http://localhost:8081/api/accounts/login \
  -H "Content-Type: application/json" \
  -d '{"cpfOrNumber":"12345678909","password":"123456"}'

# 3. Fazer dep�sito
curl -X POST http://localhost:8081/api/accounts/movements \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"type":"C","value":1000.00}'

# 4. Consultar saldo
curl -X GET http://localhost:8081/api/accounts/balance \
  -H "Authorization: Bearer {token}"

# 5. Fazer transfer�ncia
curl -X POST http://localhost:8082/api/transfers \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"destinationAccountNumber":"87654321","value":100.00}'
```

## ?? Diferenciais Implementados

- ? **Kafka** para processamento ass�ncrono de tarifas
- ? **Testes de integra��o** al�m dos unit�rios
- ? **Cache Redis** configurado
- ? **Idempot�ncia** implementada
- ? **Docker-compose** completo
- ? **Monitoramento** e logs estruturados
- ? **Swagger** completo com exemplos

## ?? Contribui��o

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudan�as
4. Push para a branch
5. Abra um Pull Request

## ?? Licen�a

Este projeto est� sob a licen�a MIT.