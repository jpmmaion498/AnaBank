@echo off
REM Script de inicialização do AnaBank para Windows

echo ?? Inicializando AnaBank...

REM Verificar se Docker está rodando
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ? Docker não está rodando. Por favor, inicie o Docker primeiro.
    exit /b 1
)

REM Criar diretórios necessários
echo ?? Criando diretórios...
if not exist "deploy\data" mkdir deploy\data
if not exist "test-results" mkdir test-results
if not exist "coverage" mkdir coverage

REM Verificar se os bancos existem, senão criar
if not exist "deploy\data\anabank_accounts.db" (
    echo ??? Criando banco de dados Accounts...
    sqlite3 deploy\data\anabank_accounts.db < config\Scripts\accounts-sqlite.sql
)

if not exist "deploy\data\anabank_transfers.db" (
    echo ??? Criando banco de dados Transfers...
    sqlite3 deploy\data\anabank_transfers.db < config\Scripts\transfers-sqlite.sql
)

if not exist "deploy\data\anabank_fees.db" (
    echo ??? Criando banco de dados Fees...
    sqlite3 deploy\data\anabank_fees.db < config\Scripts\fees-sqlite.sql
)

REM Compilar a solução
echo ?? Compilando solução...
dotnet build

if %errorlevel% neq 0 (
    echo ? Falha na compilação. Abortando.
    exit /b 1
)

REM Executar testes
echo ?? Executando testes...
dotnet test --filter "UnitTests"

REM Subir containers
echo ?? Subindo containers Docker...
cd deploy
docker-compose up -d

REM Aguardar serviços ficarem prontos
echo ? Aguardando serviços ficarem prontos...
timeout /t 10 /nobreak > nul

echo.
echo ?? AnaBank inicializado com sucesso!
echo.
echo ?? URLs disponíveis:
echo    • Accounts API: http://localhost:8081
echo    • Transfers API: http://localhost:8082
echo    • Swagger Accounts: http://localhost:8081
echo    • Swagger Transfers: http://localhost:8082
echo.
echo ?? Comandos úteis:
echo    • Ver logs: docker-compose logs -f
echo    • Parar tudo: docker-compose down
echo    • Status: docker-compose ps
echo.