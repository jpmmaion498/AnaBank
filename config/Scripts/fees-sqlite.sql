-- Script para criação das tabelas do serviço Fees (Tarifas)
-- SQLite - Estrutura conforme documento original

CREATE TABLE IF NOT EXISTS tarifa (
    idtarifa TEXT(37) PRIMARY KEY, -- identificacao unica da tarifa
    idcontacorrente TEXT(37) NOT NULL, -- identificacao unica da conta corrente
    datamovimento TEXT(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
    valor REAL NOT NULL -- valor da tarifa. Usar duas casas decimais.
);

-- Índices para performance
CREATE INDEX IF NOT EXISTS idx_tarifa_conta ON tarifa(idcontacorrente);
CREATE INDEX IF NOT EXISTS idx_tarifa_data ON tarifa(datamovimento);