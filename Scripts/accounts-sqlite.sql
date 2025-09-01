-- Script para criação das tabelas do serviço Accounts
-- SQLite - Estrutura conforme documento original

-- Tabela de contas correntes
CREATE TABLE IF NOT EXISTS contacorrente (
    idcontacorrente TEXT(37) PRIMARY KEY, -- id da conta corrente
    numero INTEGER(10) NOT NULL UNIQUE, -- numero da conta corrente
    nome TEXT(100) NOT NULL, -- nome do titular da conta corrente
    cpf TEXT(11) NOT NULL UNIQUE, -- CPF do titular (campo adicional para validação)
    ativo INTEGER(1) NOT NULL default 1, -- indicativo se a conta esta ativa. (0 = inativa, 1 = ativa).
    senha TEXT(100) NOT NULL,
    salt TEXT(100) NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (ativo in (0,1))
);

-- Tabela de movimentos
CREATE TABLE IF NOT EXISTS movimento (
    idmovimento TEXT(37) PRIMARY KEY, -- identificacao unica do movimento
    idcontacorrente TEXT(37) NOT NULL, -- identificacao unica da conta corrente
    datamovimento TEXT(25) NOT NULL, -- data do movimento no formato DD/MM/YYYY
    tipomovimento TEXT(1) NOT NULL, -- tipo do movimento. (C = Credito, D = Debito).
    valor REAL NOT NULL, -- valor do movimento. Usar duas casas decimais.
    CHECK (tipomovimento in ('C','D')),
    FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

-- Tabela de idempotência
CREATE TABLE IF NOT EXISTS idempotencia (
    chave_idempotencia TEXT(37) PRIMARY KEY, -- identificacao chave de idempotencia
    requisicao TEXT(1000), -- dados de requisicao
    resultado TEXT(1000) -- dados de retorno
);

-- Índices para performance
CREATE INDEX IF NOT EXISTS idx_movimento_conta ON movimento(idcontacorrente);
CREATE INDEX IF NOT EXISTS idx_movimento_data ON movimento(datamovimento);
CREATE INDEX IF NOT EXISTS idx_contacorrente_cpf ON contacorrente(cpf);
CREATE INDEX IF NOT EXISTS idx_contacorrente_numero ON contacorrente(numero);