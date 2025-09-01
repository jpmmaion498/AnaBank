# Guia de Troubleshooting - Renderização GitHub

## Problema: Caracteres ??? no README.md

### Causa Raiz
O GitHub tem problemas com alguns caracteres Unicode específicos, principalmente:
- Emojis de pasta (??)
- Emojis de engrenagem (??)
- Símbolos especiais (??? ??? ?)
- Caracteres de drawing box

### Solução Aplicada

#### ? ANTES (Problemático)
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
- **src/** - Código fonte
  - **Accounts/** - Microsserviço de Contas
  - **Transfers/** - Microsserviço de Transferências
- **tests/** - Testes automatizados
- **deploy/** - Docker e deployment
```

## Verificação de Compatibilidade

### ? Funcionam no GitHub
- Headers: `#`, `##`, `###`
- Listas: `-`, `*`, `1.`
- Códigos: \`\`\`
- Tabelas: `|`
- Links: `[texto](url)`
- Badges: `![Badge](url)`
- Emojis simples: ?, ?

### ? Problemáticos no GitHub
- Emojis Unicode: ??, ??, ??, ??
- Box drawing: ??? ??? ?
- Símbolos especiais: ?, ?, ?

## Status Atual dos Arquivos

### ? README.md
- Estrutura limpa
- Sem emojis problemáticos
- Compatível GitHub

### ? docs/ARCHITECTURE.md
- Documentação técnica
- Encoding correto
- Diagramas Mermaid funcionando

### ? docs/API_GUIDE.md
- Exemplos HTTP corretos
- Headers funcionando
- Tabelas renderizando

## Teste de Renderização

Para testar se está funcionando:

1. **Commit e Push** das mudanças
2. **Aguardar** alguns minutos (cache GitHub)
3. **Refresh** da página no GitHub
4. **Verificar** se `???` sumiram

## Backup de Estruturas

### Versão Minimalista
```markdown
## Estrutura

**Código Fonte:**
- src/Accounts/ - API de Contas
- src/Transfers/ - API de Transferências
- src/Fees/ - Worker de Tarifas

**Testes:**
- tests/AnaBank.Accounts.UnitTests/
- tests/AnaBank.Transfers.UnitTests/

**Deploy:**
- deploy/docker-compose.yml
- deploy/nginx/
```

### Versão com Indentação
```markdown
## Estrutura

    AnaBank/
        src/
            Accounts/           # Microsserviço de Contas
            Transfers/          # Microsserviço de Transferências
            Fees/               # Worker de Tarifas
        tests/                  # Testes automatizados
        deploy/                 # Docker e deployment
        docs/                   # Documentação
```

## Recomendação Final

**Use sempre:**
- Markdown padrão
- ASCII simples
- Sem emojis Unicode
- Estruturas com indentação ou listas