# AnaBank - Sistema Bancario Digital

## EXECUCAO RAPIDA

### Pre-requisito:
- Docker Desktop ([Download](https://www.docker.com/products/docker-desktop/))

### Iniciar Sistema:
```bash
.\INICIAR-ANABANK.bat
```

### Testar no Postman:
1. Importe: `AnaBank-Complete.postman_collection.json`
2. Importe: `AnaBank-Production.postman_environment.json`
3. Execute a collection completa

---

## URLs DO SISTEMA

| Servico | URL | Porta |
|---------|-----|-------|
| **Accounts API** | http://localhost:8091/swagger | 8091 |
| **Transfers API** | http://localhost:8092/swagger | 8092 |
| **Health Accounts** | http://localhost:8091/health | 8091 |
| **Health Transfers** | http://localhost:8092/health | 8092 |

---

## FLUXO DE TESTE AUTOMATIZADO

A collection executa automaticamente:

### 1. Setup e Validacao
- Health check das APIs
- Verificacao de conectividade

### 2. Fluxo Usuario Origem
- Cadastro de usuario
- Login (captura token automatico)
- Deposito R$ 5.000
- Consulta saldo inicial

### 3. Fluxo Usuario Destino
- Cadastro de usuario
- Login (captura token automatico)
- Deposito R$ 1.000
- Consulta saldo inicial

### 4. Transferencias
- Usuario 1 transfere R$ 100 para Usuario 2
- Usuario 1 transfere R$ 250 para Usuario 2
- Usuario 1 transfere R$ 150 para Usuario 2
- Worker processa tarifas via Kafka

### 5. Validacao Final
- Usuario 1: R$ 4.494 (5000 - 500 transferencias - 6 tarifas)
- Usuario 2: R$ 1.500 (1000 + 500 recebimentos)

---

## MONITORAMENTO

### Logs em tempo real:
```bash
docker-compose -f docker-compose.production.yml logs -f
```

### Status dos containers:
```bash
docker-compose -f docker-compose.production.yml ps
```

---

## Parar Sistema:
```bash
.\PARAR-ANABANK.bat
```

---

## TECNOLOGIAS IMPLEMENTADAS

| Tecnologia | Status | Evidencia |
|------------|--------|-----------|
| **DDD + CQRS** | OK | Arquitetura implementada |
| **JWT obrigatorio** | OK | Endpoints protegidos |
| **SQLite** | OK | Bancos funcionais |
| **Worker Service** | OK | Processamento tarifas |
| **Kafka** | OK | Comunicacao assincrona |
| **Docker** | OK | Containerizacao completa |
| **Swagger** | OK | Documentacao interativa |
| **Testes** | OK | Collection automatizada |

---

**Sistema AnaBank completo funcionando! Execute o .bat e teste no Postman!**