@echo off
REM Script de inicializa��o do AnaBank para Windows

echo ?? Inicializando AnaBank...

REM Verificar se Docker est� rodando
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ? Docker n�o est� rodando. Por favor, inicie o Docker primeiro.
    exit /b 1
)

REM Criar diret�rios necess�rios
echo ?? Criando diret�rios...
if not exist "deploy\data" mkdir deploy\data
if not exist "test-results" mkdir test-results
if not exist "coverage" mkdir coverage

REM Verificar se os bancos existem, sen�o criar
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

REM Compilar a solu��o
echo ?? Compilando solu��o...
dotnet build

if %errorlevel% neq 0 (
    echo ? Falha na compila��o. Abortando.
    exit /b 1
)

REM Executar testes
echo ?? Executando testes...
dotnet test --filter "UnitTests"

REM Subir containers
echo ?? Subindo containers Docker...
cd deploy
docker-compose up -d

REM Aguardar servi�os ficarem prontos
echo ? Aguardando servi�os ficarem prontos...
timeout /t 10 /nobreak > nul

echo.
echo ?? AnaBank inicializado com sucesso!
echo.
echo ?? URLs dispon�veis:
echo    � Accounts API: http://localhost:8081
echo    � Transfers API: http://localhost:8082
echo    � Swagger Accounts: http://localhost:8081
echo    � Swagger Transfers: http://localhost:8082
echo.
echo ?? Comandos �teis:
echo    � Ver logs: docker-compose logs -f
echo    � Parar tudo: docker-compose down
echo    � Status: docker-compose ps
echo.