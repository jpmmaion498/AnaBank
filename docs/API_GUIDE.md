# AnaBank - Guia de APIs

## ?? Vis�o Geral

O AnaBank exp�e duas APIs principais:
- **Accounts API** (porta 8081): Gest�o de contas e movimenta��es
- **Transfers API** (porta 8082): Transfer�ncias entre contas

## ?? Autentica��o

Todas as APIs (exceto cadastro e login) requerem autentica��o via JWT Bearer token.

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
  "name": "Jo�o Silva",
  "cpf": "12345678909",
  "password": "123456"
}
```

**Respostas:**
- `201 Created`: Conta criada com sucesso
- `400 Bad Request`: CPF inv�lido ou dados incorretos

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
  "title": "CPF Inv�lido",
  "detail": "O CPF informado n�o � v�lido",
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
- `401 Unauthorized`: Credenciais inv�lidas

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
- `400 Bad Request`: Conta inv�lida ou inativa
- `403 Forbidden`: Token inv�lido

**Exemplo de Resposta (200):**
```json
{
  "number": "87654321",
  "name": "Jo�o Silva",
  "dateTime": "2024-01-15T10:30:00Z",
  "balance": 1500.50
}
```

---

### 4. ?? Movimenta��o (Cr�dito/D�bito)

```http
POST /api/accounts/movements
Authorization: Bearer {token}
Content-Type: application/json
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000

{
  "accountNumber": "87654321",  // Opcional (null = conta pr�pria)
  "type": "C",                  // C = Cr�dito, D = D�bito
  "value": 100.50
}
```

**Regras de Neg�cio:**
- `accountNumber` � opcional. Se n�o informado, usa a conta do token
- Se `accountNumber` diferente da conta do token, s� aceita tipo "C" (cr�dito)
- Para d�bito pr�prio, verifica saldo suficiente

**Respostas:**
- `204 No Content`: Movimenta��o realizada
- `400 Bad Request`: Dados inv�lidos
- `403 Forbidden`: Token inv�lido

**Tipos de Erro:**
- `INVALID_ACCOUNT`: Conta n�o existe
- `INACTIVE_ACCOUNT`: Conta inativa
- `INVALID_VALUE`: Valor <= 0
- `INVALID_TYPE`: Tipo diferente de C ou D
- `INSUFFICIENT_FUNDS`: Saldo insuficiente para d�bito

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
- `403 Forbidden`: Token inv�lido

---

## ?? Transfers API

### Base URL: `http://localhost:8082/api/transfers`

---

### 1. ?? Realizar Transfer�ncia

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

**Fluxo da Transfer�ncia:**
1. Valida dados da transfer�ncia
2. Debita valor da conta de origem (via Accounts API)
3. Credita valor na conta de destino (via Accounts API)
4. Em caso de falha no cr�dito, faz estorno autom�tico
5. Registra transfer�ncia no banco
6. Publica evento no Kafka (opcional - para tarifas)

**Respostas:**
- `204 No Content`: Transfer�ncia realizada
- `400 Bad Request`: Dados inv�lidos ou falha na opera��o
- `403 Forbidden`: Token inv�lido

**Tipos de Erro:**
- `INVALID_ACCOUNT`: Conta de destino n�o existe
- `INACTIVE_ACCOUNT`: Conta inativa
- `INVALID_VALUE`: Valor <= 0
- `INSUFFICIENT_FUNDS`: Saldo insuficiente

---

## ?? Headers Especiais

### Idempot�ncia
Para opera��es cr�ticas (movimenta��es, transfer�ncias):

```http
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000
```

- Use UUIDs �nicos
- Permite retry seguro de opera��es
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

### Verificar Sa�de dos Servi�os

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

## ?? C�digos de Status HTTP

| C�digo | Significado | Uso |
|--------|-------------|-----|
| 200 | OK | Consultas bem-sucedidas |
| 201 | Created | Cadastro de conta |
| 204 | No Content | Opera��es sem retorno |
| 400 | Bad Request | Dados inv�lidos |
| 401 | Unauthorized | Credenciais inv�lidas |
| 403 | Forbidden | Token inv�lido/expirado |
| 429 | Too Many Requests | Rate limit excedido |
| 500 | Internal Server Error | Erro interno |

---

## ?? Exemplos de Fluxo Completo

### Fluxo de Cadastro e Transfer�ncia

```bash
# 1. Cadastrar primeira conta
curl -X POST http://localhost:8081/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"name":"Ana Silva","cpf":"12345678909","password":"123456"}'

# Resposta: {"id":"abc123","number":"11111111"}

# 2. Cadastrar segunda conta
curl -X POST http://localhost:8081/api/accounts \
  -H "Content-Type: application/json" \
  -d '{"name":"Jo�o Santos","cpf":"98765432100","password":"654321"}'

# Resposta: {"id":"def456","number":"22222222"}

# 3. Login primeira conta
curl -X POST http://localhost:8081/api/accounts/login \
  -H "Content-Type: application/json" \
  -d '{"cpfOrNumber":"12345678909","password":"123456"}'

# Resposta: {"token":"eyJhbGciOiJIUzI1NiJ9..."}

# 4. Fazer dep�sito inicial
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

# 7. Verificar saldo ap�s transfer�ncia
curl -X GET http://localhost:8081/api/accounts/balance \
  -H "Authorization: Bearer {TOKEN}"

# Resposta: {"number":"11111111","name":"Ana Silva","dateTime":"...","balance":900.00}
```

---

## ?? Troubleshooting

### Problemas Comuns

**401 Unauthorized**
- Verificar se o token est� correto
- Token pode ter expirado (24h por padr�o)
- Fazer novo login

**400 Bad Request com INVALID_DOCUMENT**
- CPF deve ser v�lido (11 d�gitos, algoritmo brasileiro)
- Remover pontos e tra�os do CPF

**400 Bad Request com INSUFFICIENT_FUNDS**
- Verificar saldo antes de d�bitos/transfer�ncias
- Fazer dep�sito se necess�rio

**403 Forbidden**
- Token malformado ou inv�lido
- Verificar header Authorization

**Rate Limit (429)**
- Aguardar tempo de reset
- Implementar backoff exponencial no cliente