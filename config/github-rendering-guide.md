# Guia de Troubleshooting - Renderiza��o GitHub

## Problema: Caracteres ??? no README.md

### Causa Raiz
O GitHub tem problemas com alguns caracteres Unicode espec�ficos, principalmente:
- Emojis de pasta (??)
- Emojis de engrenagem (??)
- S�mbolos especiais (??? ??? ?)
- Caracteres de drawing box

### Solu��o Aplicada

#### ? ANTES (Problem�tico)
```markdown
## ?? Estrutura do Projeto
```
AnaBank/
??? ?? src/
?   ??? ?? Accounts/
?   ??? ?? appsettings.json
```

#### ? DEPOIS (Funcionando)
```markdown
## Estrutura do Projeto
```
AnaBank/
??? src/
?   ??? Accounts/
?   ??? appsettings.json
```

### Alternativas Testadas

#### 1. Estrutura com Tree ASCII
```
AnaBank/
|
+-- src/
|   +-- Accounts/
|   +-- Transfers/
+-- tests/
```

#### 2. Estrutura Simples
```
AnaBank/
  src/
    Accounts/
    Transfers/
  tests/
```

#### 3. Lista com Bullet Points
```markdown
- **src/** - C�digo fonte
  - **Accounts/** - Microsservi�o de Contas
  - **Transfers/** - Microsservi�o de Transfer�ncias
- **tests/** - Testes automatizados
- **deploy/** - Docker e deployment
```

## Verifica��o de Compatibilidade

### ? Funcionam no GitHub
- Headers: `#`, `##`, `###`
- Listas: `-`, `*`, `1.`
- C�digos: \`\`\`
- Tabelas: `|`
- Links: `[texto](url)`
- Badges: `![Badge](url)`
- Emojis simples: ?, ?

### ? Problem�ticos no GitHub
- Emojis Unicode: ??, ??, ??, ??
- Box drawing: ??? ??? ?
- S�mbolos especiais: ?, ?, ?

## Status Atual dos Arquivos

### ? README.md
- Estrutura limpa
- Sem emojis problem�ticos
- Compat�vel GitHub

### ? docs/ARCHITECTURE.md
- Documenta��o t�cnica
- Encoding correto
- Diagramas Mermaid funcionando

### ? docs/API_GUIDE.md
- Exemplos HTTP corretos
- Headers funcionando
- Tabelas renderizando

## Teste de Renderiza��o

Para testar se est� funcionando:

1. **Commit e Push** das mudan�as
2. **Aguardar** alguns minutos (cache GitHub)
3. **Refresh** da p�gina no GitHub
4. **Verificar** se `???` sumiram

## Backup de Estruturas

### Vers�o Minimalista
```markdown
## Estrutura

**C�digo Fonte:**
- src/Accounts/ - API de Contas
- src/Transfers/ - API de Transfer�ncias
- src/Fees/ - Worker de Tarifas

**Testes:**
- tests/AnaBank.Accounts.UnitTests/
- tests/AnaBank.Transfers.UnitTests/

**Deploy:**
- deploy/docker-compose.yml
- deploy/nginx/
```

### Vers�o com Indenta��o
```markdown
## Estrutura

    AnaBank/
        src/
            Accounts/           # Microsservi�o de Contas
            Transfers/          # Microsservi�o de Transfer�ncias
            Fees/               # Worker de Tarifas
        tests/                  # Testes automatizados
        deploy/                 # Docker e deployment
        docs/                   # Documenta��o
```

## Recomenda��o Final

**Use sempre:**
- Markdown padr�o
- ASCII simples
- Sem emojis Unicode
- Estruturas com indenta��o ou listas