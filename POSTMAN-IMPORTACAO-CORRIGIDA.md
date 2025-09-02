# ?? **GUIA DE IMPORTA��O POSTMAN - CORRIGIDO**

## ? **PROBLEMA RESOLVIDO**

Os arquivos do Postman foram **recreados com encoding correto** para resolver o erro de importa��o.

---

## ?? **ARQUIVOS CORRIGIDOS**

### **?? Collection:**
- **Arquivo**: `AnaBank-Complete.postman_collection.json`
- **Problema**: Caracteres UTF-8 incorretos causando erro de parse
- **Corre��o**: Recriado com encoding ASCII/UTF-8 compat�vel
- **Status**: ? **Pronto para importa��o**

### **?? Environment:**
- **Arquivo**: `AnaBank-Production.postman_environment.json`
- **Problema**: Estrutura JSON malformada
- **Corre��o**: Recriado com estrutura v�lida
- **Status**: ? **Pronto para importa��o**

---

## ?? **INSTRU��ES DE IMPORTA��O**

### **Passo 1: Importar Collection**
1. **Abra o Postman**
2. **Clique em "Import"** (bot�o no canto superior esquerdo)
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
2. **Clique no bot�o "Run"** (??)
3. **Configure**:
   - ? Iterations: 1
   - ? Delay: 1000ms
   - ? Data File: (deixar em branco)
4. **Clique "Run AnaBank - Sistema Completo"**

---

## ?? **CONTE�DO DA COLLECTION**

### **01 - SETUP E VALIDA��O**
- ? Health Check - Accounts API
- ? Health Check - Transfers API

### **02 - USU�RIO ORIGEM**
- ? Cadastrar Usu�rio Origem (Carlos Silva)
- ? Login Usu�rio Origem
- ? Dep�sito R$ 5.000
- ? Consultar Saldo Inicial

### **03 - USU�RIO DESTINO**
- ? Cadastrar Usu�rio Destino (Maria Santos)
- ? Login Usu�rio Destino
- ? Dep�sito R$ 1.000
- ? Consultar Saldo Inicial

### **04 - TRANSFER�NCIAS**
- ? Transfer�ncia 1 - R$ 100
- ? Transfer�ncia 2 - R$ 250
- ? Transfer�ncia 3 - R$ 150
- ? Aguardar Processamento Kafka (10s)

### **05 - VALIDA��O FINAL**
- ? Saldo Final Usu�rio Origem (esperado: R$ 4.494)
- ? Saldo Final Usu�rio Destino (esperado: R$ 1.500)
- ? Resumo Final do Sistema

---

## ?? **VARI�VEIS DO ENVIRONMENT**

### **URLs:**
- `accounts_url`: http://localhost:8091
- `transfers_url`: http://localhost:8092

### **Tokens (preenchidos automaticamente):**
- `user1_token`: JWT do usu�rio origem
- `user2_token`: JWT do usu�rio destino

### **IDs (capturados automaticamente):**
- `user1_account_id`: ID da conta origem
- `user1_account_number`: N�mero da conta origem
- `user2_account_id`: ID da conta destino
- `user2_account_number`: N�mero da conta destino

### **Saldos (calculados automaticamente):**
- `user1_balance_initial`: Saldo inicial usu�rio origem
- `user1_balance_final`: Saldo final usu�rio origem
- `user2_balance_initial`: Saldo inicial usu�rio destino
- `user2_balance_final`: Saldo final usu�rio destino

---

## ? **RESULTADO ESPERADO**

### **Console Final:**
```
===============================================
?? SISTEMA ANABANK FINALIZADO COM SUCESSO!
===============================================

?? USU�RIO ORIGEM:
   Saldo inicial: R$ 5000
   Saldo final:   R$ 4494
   Diferen�a:     R$ -506

?? USU�RIO DESTINO:
   Saldo inicial: R$ 1000
   Saldo final:   R$ 1500
   Diferen�a:     R$ 500

?? OPERA��ES REALIZADAS:
   ? 3 Transfer�ncias (R$ 100 + R$ 250 + R$ 150 = R$ 500)
   ? 3 Tarifas processadas via Kafka (3 � R$ 2 = R$ 6)
   ? Worker funcionando corretamente
   ? JWT Authentication funcionando
   ? APIs Accounts + Transfers funcionando

?? SISTEMA ANABANK 100% FUNCIONAL!
```

---

## ?? **SOLU��O DE PROBLEMAS**

### **? Collection n�o importa:**
- Verifique se o arquivo `AnaBank-Complete.postman_collection.json` n�o est� corrompido
- Tente importar via "Link" usando o caminho completo do arquivo
- Abra o arquivo em um editor de texto para verificar se � JSON v�lido

### **? Environment n�o aparece:**
- Certifique-se de ter importado `AnaBank-Production.postman_environment.json`
- Verifique se selecionou o environment no dropdown superior direito
- Reiniicie o Postman se necess�rio

### **? Testes falham:**
- Certifique-se de que o sistema est� rodando: `.\INICIAR-ANABANK.bat`
- Verifique se as URLs est�o corretas (localhost:8091 e localhost:8092)
- Aguarde as APIs estarem online (pode demorar 1-2 minutos)

---

## ?? **SUCESSO!**

**Os arquivos Postman foram corrigidos e agora devem importar sem problemas!**

1. ? **Collection**: JSON v�lido, encoding correto
2. ? **Environment**: Estrutura correta, vari�veis definidas
3. ? **Tests**: Scripts funcionais, logs informativos
4. ? **Automa��o**: Captura autom�tica de tokens e IDs

**Agora voc� pode testar o sistema AnaBank completo no Postman!**