# ?? **GUIA DE IMPORTAÇÃO POSTMAN - CORRIGIDO**

## ? **PROBLEMA RESOLVIDO**

Os arquivos do Postman foram **recreados com encoding correto** para resolver o erro de importação.

---

## ?? **ARQUIVOS CORRIGIDOS**

### **?? Collection:**
- **Arquivo**: `AnaBank-Complete.postman_collection.json`
- **Problema**: Caracteres UTF-8 incorretos causando erro de parse
- **Correção**: Recriado com encoding ASCII/UTF-8 compatível
- **Status**: ? **Pronto para importação**

### **?? Environment:**
- **Arquivo**: `AnaBank-Production.postman_environment.json`
- **Problema**: Estrutura JSON malformada
- **Correção**: Recriado com estrutura válida
- **Status**: ? **Pronto para importação**

---

## ?? **INSTRUÇÕES DE IMPORTAÇÃO**

### **Passo 1: Importar Collection**
1. **Abra o Postman**
2. **Clique em "Import"** (botão no canto superior esquerdo)
3. **Selecione "Upload Files"**
4. **Escolha**: `AnaBank-Complete.postman_collection.json`
5. **Clique "Import"**
6. **Verifique** se apareceu "AnaBank - Sistema Completo" na sidebar

### **Passo 2: Importar Environment**
1. **Clique em "Import"** novamente
2. **Selecione "Upload Files"**
3. **Escolha**: `AnaBank-Production.postman_environment.json`
4. **Clique "Import"**
5. **Selecione o environment** "AnaBank - Production" no dropdown (canto superior direito)

### **Passo 3: Executar Testes**
1. **Clique na collection** "AnaBank - Sistema Completo"
2. **Clique no botão "Run"** (??)
3. **Configure**:
   - ? Iterations: 1
   - ? Delay: 1000ms
   - ? Data File: (deixar em branco)
4. **Clique "Run AnaBank - Sistema Completo"**

---

## ?? **CONTEÚDO DA COLLECTION**

### **01 - SETUP E VALIDAÇÃO**
- ? Health Check - Accounts API
- ? Health Check - Transfers API

### **02 - USUÁRIO ORIGEM**
- ? Cadastrar Usuário Origem (Carlos Silva)
- ? Login Usuário Origem
- ? Depósito R$ 5.000
- ? Consultar Saldo Inicial

### **03 - USUÁRIO DESTINO**
- ? Cadastrar Usuário Destino (Maria Santos)
- ? Login Usuário Destino
- ? Depósito R$ 1.000
- ? Consultar Saldo Inicial

### **04 - TRANSFERÊNCIAS**
- ? Transferência 1 - R$ 100
- ? Transferência 2 - R$ 250
- ? Transferência 3 - R$ 150
- ? Aguardar Processamento Kafka (10s)

### **05 - VALIDAÇÃO FINAL**
- ? Saldo Final Usuário Origem (esperado: R$ 4.494)
- ? Saldo Final Usuário Destino (esperado: R$ 1.500)
- ? Resumo Final do Sistema

---

## ?? **VARIÁVEIS DO ENVIRONMENT**

### **URLs:**
- `accounts_url`: http://localhost:8091
- `transfers_url`: http://localhost:8092

### **Tokens (preenchidos automaticamente):**
- `user1_token`: JWT do usuário origem
- `user2_token`: JWT do usuário destino

### **IDs (capturados automaticamente):**
- `user1_account_id`: ID da conta origem
- `user1_account_number`: Número da conta origem
- `user2_account_id`: ID da conta destino
- `user2_account_number`: Número da conta destino

### **Saldos (calculados automaticamente):**
- `user1_balance_initial`: Saldo inicial usuário origem
- `user1_balance_final`: Saldo final usuário origem
- `user2_balance_initial`: Saldo inicial usuário destino
- `user2_balance_final`: Saldo final usuário destino

---

## ? **RESULTADO ESPERADO**

### **Console Final:**
```
===============================================
?? SISTEMA ANABANK FINALIZADO COM SUCESSO!
===============================================

?? USUÁRIO ORIGEM:
   Saldo inicial: R$ 5000
   Saldo final:   R$ 4494
   Diferença:     R$ -506

?? USUÁRIO DESTINO:
   Saldo inicial: R$ 1000
   Saldo final:   R$ 1500
   Diferença:     R$ 500

?? OPERAÇÕES REALIZADAS:
   ? 3 Transferências (R$ 100 + R$ 250 + R$ 150 = R$ 500)
   ? 3 Tarifas processadas via Kafka (3 × R$ 2 = R$ 6)
   ? Worker funcionando corretamente
   ? JWT Authentication funcionando
   ? APIs Accounts + Transfers funcionando

?? SISTEMA ANABANK 100% FUNCIONAL!
```

---

## ?? **SOLUÇÃO DE PROBLEMAS**

### **? Collection não importa:**
- Verifique se o arquivo `AnaBank-Complete.postman_collection.json` não está corrompido
- Tente importar via "Link" usando o caminho completo do arquivo
- Abra o arquivo em um editor de texto para verificar se é JSON válido

### **? Environment não aparece:**
- Certifique-se de ter importado `AnaBank-Production.postman_environment.json`
- Verifique se selecionou o environment no dropdown superior direito
- Reiniicie o Postman se necessário

### **? Testes falham:**
- Certifique-se de que o sistema está rodando: `.\INICIAR-ANABANK.bat`
- Verifique se as URLs estão corretas (localhost:8091 e localhost:8092)
- Aguarde as APIs estarem online (pode demorar 1-2 minutos)

---

## ?? **SUCESSO!**

**Os arquivos Postman foram corrigidos e agora devem importar sem problemas!**

1. ? **Collection**: JSON válido, encoding correto
2. ? **Environment**: Estrutura correta, variáveis definidas
3. ? **Tests**: Scripts funcionais, logs informativos
4. ? **Automação**: Captura automática de tokens e IDs

**Agora você pode testar o sistema AnaBank completo no Postman!**