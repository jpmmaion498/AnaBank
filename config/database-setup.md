# Configuração de Bancos de Dados - AnaBank

## Estrutura dos Bancos

### Accounts Database (anabank_accounts.db)
- **contacorrente**: Dados das contas correntes
- **movimento**: Movimentações financeiras (créditos/débitos)
- **idempotencia**: Controle de requisições duplicadas

### Transfers Database (anabank_transfers.db)
- **transferencia**: Histórico de transferências
- **idempotencia**: Controle de requisições duplicadas

### Fees Database (anabank_fees.db)
- **tarifa**: Tarifas processadas pelo worker

## Inicialização

Os scripts SQL estão em `config/Scripts/` e são executados automaticamente na inicialização dos containers.

## Backup e Restore

### Backup
```bash
# Backup manual
sqlite3 deploy/data/anabank_accounts.db ".backup accounts_backup.db"
sqlite3 deploy/data/anabank_transfers.db ".backup transfers_backup.db"
sqlite3 deploy/data/anabank_fees.db ".backup fees_backup.db"
```

### Restore
```bash
# Restore manual
sqlite3 deploy/data/anabank_accounts.db ".restore accounts_backup.db"
```

## Migração de Dados

Para mudanças de schema, criar scripts em `config/Scripts/migrations/`:
- `001_add_index_cpf.sql`
- `002_add_status_column.sql`
- etc.