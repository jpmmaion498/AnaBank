# ?? AnaBank - Guia Rapido para Avaliacao

## ? **EXECUCAO EM 30 SEGUNDOS**

### **1. Executar Sistema**
```bash
.\INICIAR-AVALIACAO.bat
```
*Aguarde aparecer "SISTEMA PRONTO PARA AVALIACAO!"*

### **2. Importar no Postman**
1. **Collection**: `AnaBank-Avaliacao-Final.postman_collection.json`
2. **Environment**: `AnaBank-Avaliacao-Final.postman_environment.json`

### **3. Executar Testes**
- Clique em **"Run Collection"**
- Execute **todos os requests**
- Observe resultados no **Console**

---

## ?? **RESULTADO ESPERADO**

```
?? TESTE ANABANK FINALIZADO COM SUCESSO!
===============================================

?? ANA SILVA:
   Saldo inicial: R$ 5000
   Saldo final:   R$ 4494
   Diferenca:     R$ -506

?? JOAO SANTOS:
   Saldo inicial: R$ 1000
   Saldo final:   R$ 1500
   Diferenca:     R$ 500

?? OPERACOES REALIZADAS:
   ? 3 Transferencias (R$ 500 total)
   ? 3 Tarifas via Kafka (R$ 6 total)
   ? Worker funcionando
   ? JWT Authentication OK
   ? APIs funcionando

?? SISTEMA ANABANK 100% FUNCIONAL!
```

---

## ?? **URLS DO SISTEMA**

| Servico | URL | Funcao |
|---------|-----|--------|
| **Accounts** | http://localhost:8091/swagger | API de contas |
| **Transfers** | http://localhost:8092/swagger | API de transferencias |
| **Health** | http://localhost:8091/health | Status da API |

---

## ? **VALIDACAO COMPLETA**

### **Requisitos Obrigatorios:**
- ? **DDD + CQRS** - Arquitetura implementada
- ? **JWT obrigatorio** - Autenticacao em todas APIs  
- ? **SQLite** - Banco de dados funcional
- ? **Worker Service** - Processamento de tarifas
- ? **Testes** - Collection automatizada

### **Diferenciais:**
- ? **Kafka** - Comunicacao assincrona
- ? **Docker** - Containerizacao completa
- ? **Swagger** - Documentacao interativa
- ? **Health Checks** - Monitoramento
- ? **Idempotencia** - Headers implementados

---

## ?? **PROBLEMAS?**

### **? APIs nao respondem**
```bash
# Ver logs
docker-compose -f docker-compose.avaliacao.yml logs accounts-api

# Aguardar mais tempo
# Kafka pode demorar para inicializar
```

### **? Collection falha**
- Verificar se Environment esta selecionado
- Aguardar APIs estarem online
- Executar requests individualmente

---

**?? Sistema profissional pronto para avaliacao!**