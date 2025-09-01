# Configura��o de Bancos de Dados - AnaBank

## Estrutura dos Bancos

### Accounts Database (anabank_accounts.db)
- **contacorrente**: Dados das contas correntes
- **movimento**: Movimenta��es financeiras (cr�ditos/d�bitos)
- **idempotencia**: Controle de requisi��es duplicadas

### Transfers Database (anabank_transfers.db)
- **transferencia**: Hist�rico de transfer�ncias
- **idempotencia**: Controle de requisi��es duplicadas

### Fees Database (anabank_fees.db)
- **tarifa**: Tarifas processadas pelo worker

## Inicializa��o

Os scripts SQL est�o em `config/Scripts/` e s�o executados automaticamente na inicializa��o dos containers.

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

## Migra��o de Dados

Para mudan�as de schema, criar scripts em `config/Scripts/migrations/`:
- `001_add_index_cpf.sql`
- `002_add_status_column.sql`
- etc.