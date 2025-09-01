# AnaBank - Guia de APIs

## ?? Visão Geral

O AnaBank expõe duas APIs principais:
- **Accounts API** (porta 8081): Gestão de contas e movimentações
- **Transfers API** (porta 8082): Transferências entre contas

## ?? Autenticação

Todas as APIs (exceto cadastro e login) requerem autenticação via JWT Bearer token.

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## ?? Accounts API

### Base URL: `http://localhost:8081/api/accounts`

---

### 1. ?? Cadastrar Conta

```http
POST /api/accounts
Content-Type: application/json

{
  "name": "João Silva",
  "cpf": "12345678909",
  "password": "123456"
}
```

**Respostas:**
- `201 Created`: Conta criada com sucesso
- `400 Bad Request`: CPF inválido ou dados incorretos

**Exemplo de Resposta (201):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "number": "87654321"
}
```

**Exemplo de Erro (400):**
```json
{
  "type": "INVALID_DOCUMENT",
  "title": "CPF Inválido",
  "detail": "O CPF informado não é válido",
  "status": 400
}
```

---

### 2. ?? Login

```http
POST /api/accounts/login
Content-Type: application/json

{
  "cpfOrNumber": "12345678909",
  "password": "123456"
}
```

**Respostas:**
- `200 OK`: Login realizado com sucesso
- `401 Unauthorized`: Credenciais inválidas

**Exemplo de Resposta (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Exemplo de Erro (401):**
```json
{
  "type": "USER_UNAUTHORIZED",
  "title": "Unauthorized",
  "detail": "Invalid credentials",
  "status": 401
}
```

---

### 3. ?? Consultar Saldo

```http
GET /api/accounts/balance
Authorization: Bearer {token}
```

**Respostas:**
- `200 OK`: Saldo consultado com sucesso
- `400 Bad Request`: Conta inválida ou inativa
- `403 Forbidden`: Token inválido

**Exemplo de Resposta (200):**
```json
{
  "number": "87654321",
  "name": "João Silva",
  "dateTime": "2024-01-15T10:30:00Z",
  "balance": 1500.50
}
```

---

### 4. ?? Movimentação (Crédito/Débito)

```http
POST /api/accounts/movements
Authorization: Bearer {token}
Content-Type: application/json
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000

{
  "accountNumber": "87654321",  // Opcional (null = conta própria)
  "type": "C",                  // C = Crédito, D = Débito
  "value": 100.50
}
```

**Regras de Negócio:**
- `accountNumber` é opcional. Se não informado, usa a conta do token
- Se `accountNumber` diferente da conta do token, só aceita tipo "C" (crédito)
- Para débito próprio, verifica saldo suficiente

**Respostas:**
- `204 No Content`: Movimentação realizada
- `400 Bad Request`: Dados inválidos
- `403 Forbidden`: Token inválido

**Tipos de Erro:**
- `INVALID_ACCOUNT`: Conta não existe
- `INACTIVE_ACCOUNT`: Conta inativa
- `INVALID_VALUE`: Valor <= 0
- `INVALID_TYPE`: Tipo diferente de C ou D
- `INSUFFICIENT_FUNDS`: Saldo insuficiente para débito

---

### 5. ?? Desativar Conta

```http
POST /api/accounts/deactivate
Authorization: Bearer {token}
Content-Type: application/json

{
  "password": "123456"
}
```

**Respostas:**
- `204 No Content`: Conta desativada
- `401 Unauthorized`: Senha incorreta
- `403 Forbidden`: Token inválido

---

## ?? Transfers API

### Base URL: `http://localhost:8082/api/transfers`

---

### 1. ?? Realizar Transferência

```http
POST /api/transfers
Authorization: Bearer {token}
Content-Type: application/json
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440001

{
  "destinationAccountNumber": "87654321",
  "value": 100.50
}
```

**Fluxo da Transferência:**
1. Valida dados da transferência
2. Debita valor da conta de origem (via Accounts API)
3. Credita valor na conta de destino (via Accounts API)
4. Em caso de falha no crédito, faz estorno automático
5. Registra transferência no banco
6. Publica evento no Kafka (opcional - para tarifas)

**Respostas:**
- `204 No Content`: Transferência realizada
- `400 Bad Request`: Dados inválidos ou falha na operação
- `403 Forbidden`: Token inválido

**Tipos de Erro:**
- `INVALID_ACCOUNT`: Conta de destino não existe
- `INACTIVE_ACCOUNT`: Conta inativa
- `INVALID_VALUE`: Valor <= 0
- `INSUFFICIENT_FUNDS`: Saldo insuficiente

---

## ?? Headers Especiais

### Idempotência
Para operações críticas (movimentações, transferências):

```http
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000
```

- Use UUIDs únicos
- Permite retry seguro de operações
- Retorna mesma resposta para chaves repetidas

### Rate Limiting
As APIs implementam rate limiting:

```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1640995200
```

---

## ?? Health Checks

### Verificar Saúde dos Serviços

```http
GET /health
```

**Resposta:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## ?? Códigos de Status HTTP

| Código | Significado | Uso |
|--------|-------------|-----|
| 200 | OK | Consultas bem-sucedidas |
| 201 | Created | Cadastro de conta |
| 204 | No Content | Operações sem retorno |
| 400 | Bad Request | Dados inválidos |
| 401 | Unauthorized | Credenciais inválidas |
| 403 | Forbidden | Token inválido/expirado |
| 429 | Too Many Requests | Rate limit excedido |
| 500 | Internal Server Error | Erro interno |

---

## ?? Exemplos de Fluxo Completo

### Fluxo de Cadastro e Transferência

```bash
# 1. Cadastrar primeira conta
curl -X POST http://localhost:8081/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"name":"Ana Silva","cpf":"12345678909","password":"123456"}'

# Resposta: {"id":"abc123","number":"11111111"}

# 2. Cadastrar segunda conta
curl -X POST http://localhost:8081/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"name":"João Santos","cpf":"98765432100","password":"654321"}'

# Resposta: {"id":"def456","number":"22222222"}

# 3. Login primeira conta
curl -X POST http://localhost:8081/api/accounts/login \
  -H "Content-Type: application/json" \
  -d '{"cpfOrNumber":"12345678909","password":"123456"}'

# Resposta: {"token":"eyJhbGciOiJIUzI1NiJ9..."}

# 4. Fazer depósito inicial
curl -X POST http://localhost:8081/api/accounts/movements \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -H "Idempotency-Key: $(uuidgen)" \
  -d '{"type":"C","value":1000.00}'

# 5. Consultar saldo
curl -X GET http://localhost:8081/api/accounts/balance \
  -H "Authorization: Bearer {TOKEN}"

# Resposta: {"number":"11111111","name":"Ana Silva","dateTime":"...","balance":1000.00}

# 6. Transferir para segunda conta
curl -X POST http://localhost:8082/api/transfers \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -H "Idempotency-Key: $(uuidgen)" \
  -d '{"destinationAccountNumber":"22222222","value":100.00}'

# 7. Verificar saldo após transferência
curl -X GET http://localhost:8081/api/accounts/balance \
  -H "Authorization: Bearer {TOKEN}"

# Resposta: {"number":"11111111","name":"Ana Silva","dateTime":"...","balance":900.00}
```

---

## ?? Troubleshooting

### Problemas Comuns

**401 Unauthorized**
- Verificar se o token está correto
- Token pode ter expirado (24h por padrão)
- Fazer novo login

**400 Bad Request com INVALID_DOCUMENT**
- CPF deve ser válido (11 dígitos, algoritmo brasileiro)
- Remover pontos e traços do CPF

**400 Bad Request com INSUFFICIENT_FUNDS**
- Verificar saldo antes de débitos/transferências
- Fazer depósito se necessário

**403 Forbidden**
- Token malformado ou inválido
- Verificar header Authorization

**Rate Limit (429)**
- Aguardar tempo de reset
- Implementar backoff exponencial no cliente