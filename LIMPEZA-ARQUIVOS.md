# ??? Arquivos e Pastas para Limpeza da Raiz do Projeto

## ?? **VERSAO FINAL MANTIDA**
- ? `INICIAR-AVALIACAO.bat` - Script principal
- ? `PARAR-AVALIACAO.bat` - Script de parada
- ? `docker-compose.avaliacao.yml` - Docker compose final
- ? `AnaBank-Avaliacao-Final.postman_collection.json` - Collection final
- ? `AnaBank-Avaliacao-Final.postman_environment.json` - Environment final
- ? `AVALIACAO-FINAL.md` - Documentacao principal
- ? `GUIA-RAPIDO-AVALIACAO.md` - Guia rapido

---

## ??? **ARQUIVOS PARA REMOVER**

### **?? Scripts de Teste/Debug Obsoletos:**
```
check-apis.bat
clean-debug-db.bat
CORRIGIR-E-TESTAR.bat
diagnose-anabank.bat
DIAGNOSTICAR-PROBLEMA.bat
REBUILD-DOCKER.bat
quick-reset.bat
reset-databases.bat
VERIFICAR-SISTEMA.bat
```

### **?? Collections Postman Antigas:**
```
AnaBank-Basic-Test.postman_collection.json
AnaBank-Debug-Token.postman_collection.json
AnaBank-Environment.postman_environment.json
AnaBank-Postman-Collection.json
```

### **?? Scripts WSL (nao utilizados):**
```
setup-services-wsl.sh
start-wsl.sh
stop-services-wsl.sh
```

### **?? Scripts Locais Antigos:**
```
start-full-stack.bat
start-local.bat
stop-local.bat
```

### **?? Documentacao de Desenvolvimento:**
```
AVALIACAO-DOCKER.md
RESUMO-AVALIACAO.md
VALIDATION_REPORT.md
VERSAO-FINAL-RESUMO.md
```

---

## ??? **PASTAS PARA REMOVER**

### **?? teste_local_avaliador/** 
```
teste_local_avaliador/
??? apis/
??? logs/
??? scripts/
??? AnaBank-Avaliador.postman_collection.json
??? AnaBank-Avaliador.postman_environment.json
??? GUIA-RAPIDO.md
??? INICIAR-ANABANK.bat
??? INSTRUCOES.md
??? PARAR-ANABANK.bat
??? README.md
??? RESUMO-EXECUCAO.md
```
> **Motivo**: Versao antiga, substituida pela versao final

### **?? tools/** (se existir)
```
tools/
??? start.bat
??? start.sh
??? stop.sh
??? Makefile
```
> **Motivo**: Scripts de desenvolvimento, nao necessarios para avaliacao

### **?? data/** (se existir com bancos antigos)
```
data/
??? *.db
??? logs/
```
> **Motivo**: Bancos de desenvolvimento, serao criados automaticamente

---

## ?? **COMANDO PARA LIMPEZA AUTOMATICA**

```powershell
# SCRIPTS OBSOLETOS
Remove-Item -Force -ErrorAction SilentlyContinue @(
    "check-apis.bat",
    "clean-debug-db.bat", 
    "CORRIGIR-E-TESTAR.bat",
    "diagnose-anabank.bat",
    "DIAGNOSTICAR-PROBLEMA.bat",
    "REBUILD-DOCKER.bat",
    "quick-reset.bat", 
    "reset-databases.bat",
    "VERIFICAR-SISTEMA.bat",
    "setup-services-wsl.sh",
    "start-wsl.sh",
    "stop-services-wsl.sh",
    "start-full-stack.bat",
    "start-local.bat",
    "stop-local.bat"
)

# COLLECTIONS ANTIGAS
Remove-Item -Force -ErrorAction SilentlyContinue @(
    "AnaBank-Basic-Test.postman_collection.json",
    "AnaBank-Debug-Token.postman_collection.json", 
    "AnaBank-Environment.postman_environment.json",
    "AnaBank-Postman-Collection.json"
)

# DOCUMENTACAO DE DESENVOLVIMENTO
Remove-Item -Force -ErrorAction SilentlyContinue @(
    "AVALIACAO-DOCKER.md",
    "RESUMO-AVALIACAO.md", 
    "VALIDATION_REPORT.md",
    "VERSAO-FINAL-RESUMO.md"
)

# PASTAS INTEIRAS
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue @(
    "teste_local_avaliador",
    "tools"
)

# BANCOS DE DESENVOLVIMENTO (OPCIONAL)
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "data"
```

---

## ? **ARQUIVOS ESSENCIAIS MANTIDOS**

### **?? Sistema Final:**
- `INICIAR-AVALIACAO.bat`
- `PARAR-AVALIACAO.bat` 
- `docker-compose.avaliacao.yml`

### **?? Postman Final:**
- `AnaBank-Avaliacao-Final.postman_collection.json`
- `AnaBank-Avaliacao-Final.postman_environment.json`

### **?? Documentacao Final:**
- `AVALIACAO-FINAL.md`
- `GUIA-RAPIDO-AVALIACAO.md`
- `README.md` (principal)

### **?? Pastas Essenciais:**
- `src/` - Codigo fonte
- `tests/` - Testes unitarios
- `deploy/` - Docker configs 
- `docs/` - Documentacao tecnica
- `.github/` - GitHub configs

---

## ?? **RESUMO DA LIMPEZA**

### **Antes da limpeza:**
- ? **16 scripts** obsoletos
- ? **4 collections** antigas
- ? **4 documentos** de desenvolvimento  
- ? **1 pasta** teste_local_avaliador completa
- ? **1 pasta** tools (se existir)

### **Apos a limpeza:**
- ? **2 scripts** finais
- ? **1 collection** final
- ? **2 documentos** finais
- ? **Pastas essenciais** apenas

### **Reducao:**
- ?? **~25 arquivos** removidos
- ?? **~2 pastas** removidas  
- ?? **~80% menos** arquivos na raiz
- ?? **Organizacao profissional**

---

**?? Execute o comando PowerShell acima para limpeza automatica!**