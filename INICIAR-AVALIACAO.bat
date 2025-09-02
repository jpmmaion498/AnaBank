@echo off
echo ?? AnaBank - Sistema Final para Avaliacao

echo.
echo ===============================================
echo    ANABANK - SISTEMA FINAL DE AVALIACAO
echo ===============================================
echo.

echo ?? Verificando Docker...
docker --version >nul 2>&1
if errorlevel 1 (
    echo ? Docker nao encontrado!
    echo ?? Instale o Docker Desktop: https://www.docker.com/products/docker-desktop/
    pause
    exit /b 1
)
echo ? Docker encontrado

echo.
echo ?? Verificando portas...
netstat -an | findstr ":8091" >nul 2>&1
if not errorlevel 1 (
    echo ?? Porta 8091 em uso
    netstat -ano | findstr :8091
)

netstat -an | findstr ":8092" >nul 2>&1
if not errorlevel 1 (
    echo ?? Porta 8092 em uso  
    netstat -ano | findstr :8092
)

echo.
echo ?? Preparando ambiente...
docker-compose -f docker-compose.avaliacao.yml down >nul 2>&1

echo.
echo ??? Iniciando sistema completo...
echo   ?? Infraestrutura: Zookeeper + Kafka
echo   ?? APIs: Accounts (8091) + Transfers (8092)
echo   ?? Worker: Fees (background processing)
echo   ?? Load Balancer: Nginx (8090)

docker-compose -f docker-compose.avaliacao.yml up -d

if errorlevel 1 (
    echo ? Erro ao iniciar containers!
    echo ?? Verificando logs...
    docker-compose -f docker-compose.avaliacao.yml logs
    pause
    exit /b 1
)

echo.
echo ? Aguardando inicializacao (60s)...
echo   ? Kafka precisa inicializar completamente
echo   ? APIs precisam conectar ao Kafka  
echo   ? Worker precisa se conectar aos topicos
timeout /t 60 /nobreak >nul

echo.
echo ?? Verificando status dos containers...
docker-compose -f docker-compose.avaliacao.yml ps

echo.
echo ?? Testando conectividade das APIs...
echo   Accounts API (porta 8091):
powershell -Command "try { $r = Invoke-RestMethod 'http://localhost:8091/health' -TimeoutSec 10; Write-Host '   ? Accounts API: ONLINE' } catch { Write-Host '   ?? Accounts API: Ainda inicializando...' }"

echo   Transfers API (porta 8092):
powershell -Command "try { $r = Invoke-RestMethod 'http://localhost:8092/health' -TimeoutSec 10; Write-Host '   ? Transfers API: ONLINE' } catch { Write-Host '   ?? Transfers API: Ainda inicializando...' }"

echo.
echo ===============================================
echo     ?? ANABANK SISTEMA FINAL INICIADO!
echo ===============================================
echo.
echo ?? URLs para teste:
echo   • Accounts API:  http://localhost:8091/swagger
echo   • Transfers API: http://localhost:8092/swagger
echo   • Health Check:  http://localhost:8091/health
echo   • Health Check:  http://localhost:8092/health
echo.
echo ?? TESTE NO POSTMAN:
echo   1. Importe: AnaBank-Avaliacao-Final.postman_collection.json
echo   2. Importe: AnaBank-Avaliacao-Final.postman_environment.json
echo   3. Execute a collection completa
echo   4. Observe os resultados automaticos no console
echo.
echo ?? Resultados esperados:
echo   • Ana Silva: R$ 4.494 (5000 - 500 transferencias - 6 tarifas)
echo   • Joao Santos: R$ 1.500 (1000 + 500 recebimentos)
echo   • Worker processando tarifas via Kafka
echo.
echo ?? Monitoramento:
echo   • Status: docker-compose -f docker-compose.avaliacao.yml ps
echo   • Logs: docker-compose -f docker-compose.avaliacao.yml logs -f
echo.
echo ?? Para parar: .\PARAR-AVALIACAO.bat
echo.
echo ? SISTEMA PRONTO PARA AVALIACAO!
echo.
pause