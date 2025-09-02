# ?? AnaBank - Avaliacao Final

## ?? **Execucao em 1 Comando**

### **Pre-requisito**
- ? **Docker Desktop** ([Download](https://www.docker.com/products/docker-desktop/))

### **Iniciar Sistema**
```bash
.\INICIAR-AVALIACAO.bat
```

### **Testar no Postman**
1. **Importe**: `AnaBank-Avaliacao-Final.postman_collection.json`
2. **Importe**: `AnaBank-Avaliacao-Final.postman_environment.json`
3. **Execute** a collection completa

---

## ?? **URLs do Sistema**

| Servico | URL | Porta |
|---------|-----|-------|
| **Accounts API** | http://localhost:8091/swagger | 8091 |
| **Transfers API** | http://localhost:8092/swagger | 8092 |
| **Health Accounts** | http://localhost:8091/health | 8091 |
| **Health Transfers** | http://localhost:8092/health | 8092 |

---

## ?? **Fluxo de Teste Automatizado**

A collection executa automaticamente:

### **1. Setup e Validacao**
- ? Health check das APIs
- ? Verificacao de conectividade

### **2. Fluxo Ana (Conta Origem)**
- ? Cadastro da Ana Silva
- ? Login (captura token automatico)
- ? Deposito R$ 5.000
- ? Consulta saldo inicial

### **3. Fluxo Joao (Conta Destino)**
- ? Cadastro do Joao Santos
- ? Login (captura token automatico)
- ? Deposito R$ 1.000
- ? Consulta saldo inicial

### **4. Transferencias**
- ? Ana transfere R$ 100 para Joao
- ? Ana transfere R$ 250 para Joao
- ? Ana transfere R$ 150 para Joao
- ? Worker processa tarifas via Kafka

### **5. Validacao Final**
- ? Ana: R$ 4.494 (5000 - 500 transferencias - 6 tarifas)
- ? Joao: R$ 1.500 (1000 + 500 recebimentos)

---

## ?? **Monitoramento**

### **Logs em tempo real:**
```bash
docker-compose -f docker-compose.avaliacao.yml logs -f
```

### **Status dos containers:**
```bash
docker-compose -f docker-compose.avaliacao.yml ps
```

---

## ? **Parar Sistema**
```bash
.\PARAR-AVALIACAO.bat
```

---

## ? **Validacao dos Requisitos**

| Requisito | Status | Evidencia |
|-----------|--------|-----------|
| **DDD + CQRS** | ? | Arquitetura implementada |
| **JWT obrigatorio** | ? | Endpoints protegidos |
| **SQLite** | ? | Bancos funcionais |
| **Worker Service** | ? | Processamento tarifas |
| **Kafka** | ? | Comunicacao assincrona |
| **Docker** | ? | Containerizacao completa |
| **Swagger** | ? | Documentacao interativa |
| **Testes** | ? | Collection automatizada |

---

**?? Sistema completo funcionando! Execute o .bat e teste no Postman!**