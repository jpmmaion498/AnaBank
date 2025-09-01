# ?? Estrutura Corrigida - AnaBank

## ? **Organiza��o Final dos Arquivos de Configura��o**

### ?? **Arquivos `appsettings.json` (nos projetos corretos)**

```
src/
??? ?? Accounts/
?   ??? ?? AnaBank.Accounts.API/
?       ??? ?? appsettings.json              # Base
?       ??? ?? appsettings.Development.json  # Desenvolvimento
?       ??? ?? appsettings.Production.json   # Produ��o
?
??? ?? Transfers/
    ??? ?? AnaBank.Transfers.API/
        ??? ?? appsettings.json              # Base
        ??? ?? appsettings.Development.json  # Desenvolvimento
        ??? ?? appsettings.Production.json   # Produ��o
```

### ?? **Pasta `config/` (configura��es de deploy)**

```
config/
??? ?? Scripts/                      # Scripts SQL (correto)
?   ??? accounts-sqlite.sql
?   ??? transfers-sqlite.sql
?   ??? fees-sqlite.sql
??? ?? environment-variables.md      # Vari�veis de ambiente para deploy
??? ?? database-setup.md            # Documenta��o de banco
```

## ?? **Por que essa organiza��o?**

### ? **`appsettings.json` nos projetos**
- **Correto**: .NET procura esses arquivos na **mesma pasta do execut�vel**
- **Visual Studio**: Consegue adicionar como "Item Existente" ?
- **Intellisense**: Funciona corretamente ?
- **Build**: Arquivos s�o copiados automaticamente ?

### ? **`config/` para deploy**
- **Scripts SQL**: Usados na inicializa��o do Docker
- **Documenta��o**: Vari�veis de ambiente, setup de banco
- **Deploy**: Configura��es espec�ficas de infraestrutura

## ?? **Configura��es por Ambiente**

### **Development** (local)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=anabank_accounts_dev.db",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "SecretKey": "chave-de-desenvolvimento-nao-usar-em-producao-123456789",
    "ExpirationHours": 8
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "AnaBank": "Trace"
    }
  }
}
```

### **Production** (Docker)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/data/anabank_accounts.db",
    "Redis": "redis:6379"
  },
  "Jwt": {
    "SecretKey": "sua-chave-secreta-super-segura-com-pelo-menos-32-caracteres",
    "ExpirationHours": 24
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "AnaBank": "Information"
    }
  }
}
```

## ? **Agora voc� pode:**

1. **Adicionar como Item Existente** no Visual Studio ?
2. **Editar no IDE** com Intellisense ?
3. **Build autom�tico** funciona ?
4. **Deploy funciona** com Docker ?

## ?? **Comandos para testar:**

```bash
# Desenvolvimento (usa appsettings.Development.json)
cd src/Accounts/AnaBank.Accounts.API
dotnet run

# Produ��o (usa appsettings.Production.json)
cd deploy
docker-compose up -d
```