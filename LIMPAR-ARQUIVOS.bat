@echo off
echo ?? AnaBank - Limpeza Automatica da Pasta Raiz

echo.
echo ===============================================
echo    LIMPEZA AUTOMATICA DOS ARQUIVOS
echo ===============================================
echo.

echo ?? Esta operacao ira remover arquivos e pastas obsoletos
echo ?? Serao mantidos apenas os arquivos essenciais para avaliacao
echo.
echo ??? Arquivos que serao removidos:
echo   • Scripts de teste/debug obsoletos (16 arquivos)
echo   • Collections Postman antigas (4 arquivos)  
echo   • Documentacao de desenvolvimento (4 arquivos)
echo   • Pasta teste_local_avaliador/ completa
echo   • Pasta tools/ (se existir)
echo   • Bancos de desenvolvimento data/ (opcional)
echo.
echo ? Arquivos que serao mantidos:
echo   • INICIAR-AVALIACAO.bat + PARAR-AVALIACAO.bat
echo   • docker-compose.avaliacao.yml
echo   • AnaBank-Avaliacao-Final.postman_*
echo   • AVALIACAO-FINAL.md + GUIA-RAPIDO-AVALIACAO.md
echo   • Pastas essenciais: src/, tests/, deploy/, docs/
echo.

choice /c SN /m "Deseja continuar com a limpeza"
if errorlevel 2 (
    echo ? Limpeza cancelada
    pause
    exit /b 0
)

echo.
echo ??? Removendo scripts obsoletos...
del /f /q "check-apis.bat" 2>nul
del /f /q "clean-debug-db.bat" 2>nul
del /f /q "CORRIGIR-E-TESTAR.bat" 2>nul
del /f /q "diagnose-anabank.bat" 2>nul
del /f /q "DIAGNOSTICAR-PROBLEMA.bat" 2>nul
del /f /q "REBUILD-DOCKER.bat" 2>nul
del /f /q "quick-reset.bat" 2>nul
del /f /q "reset-databases.bat" 2>nul
del /f /q "VERIFICAR-SISTEMA.bat" 2>nul
del /f /q "setup-services-wsl.sh" 2>nul
del /f /q "start-wsl.sh" 2>nul
del /f /q "stop-services-wsl.sh" 2>nul
del /f /q "start-full-stack.bat" 2>nul
del /f /q "start-local.bat" 2>nul
del /f /q "stop-local.bat" 2>nul
echo   ? Scripts removidos

echo.
echo ??? Removendo collections antigas...
del /f /q "AnaBank-Basic-Test.postman_collection.json" 2>nul
del /f /q "AnaBank-Debug-Token.postman_collection.json" 2>nul
del /f /q "AnaBank-Environment.postman_environment.json" 2>nul
del /f /q "AnaBank-Postman-Collection.json" 2>nul
echo   ? Collections antigas removidas

echo.
echo ??? Removendo documentacao de desenvolvimento...
del /f /q "AVALIACAO-DOCKER.md" 2>nul
del /f /q "RESUMO-AVALIACAO.md" 2>nul
del /f /q "VALIDATION_REPORT.md" 2>nul
del /f /q "VERSAO-FINAL-RESUMO.md" 2>nul
echo   ? Documentacao de desenvolvimento removida

echo.
echo ??? Removendo pasta teste_local_avaliador/...
if exist "teste_local_avaliador" (
    rmdir /s /q "teste_local_avaliador"
    echo   ? Pasta teste_local_avaliador removida
) else (
    echo   ?? Pasta teste_local_avaliador nao encontrada
)

echo.
echo ??? Removendo pasta tools/ (se existir)...
if exist "tools" (
    rmdir /s /q "tools"
    echo   ? Pasta tools removida
) else (
    echo   ?? Pasta tools nao encontrada
)

echo.
echo ??? Remover bancos de desenvolvimento data/?
choice /c SN /m "Remover pasta data/ com bancos antigos"
if not errorlevel 2 (
    if exist "data" (
        rmdir /s /q "data"
        echo   ? Pasta data removida
    ) else (
        echo   ?? Pasta data nao encontrada
    )
) else (
    echo   ?? Pasta data mantida
)

echo.
echo ?? Verificando estrutura final...
echo.
echo ?? Arquivos mantidos na raiz:
dir /b *.bat *.yml *.json *.md 2>nul

echo.
echo ?? Pastas mantidas:
for /d %%d in (*) do echo   ?? %%d

echo.
echo ===============================================
echo     ? LIMPEZA CONCLUIDA COM SUCESSO!
echo ===============================================
echo.
echo ?? Estrutura final otimizada:
echo   • Apenas arquivos essenciais para avaliacao
echo   • Pastas de codigo fonte mantidas
echo   • Documentacao final organizada
echo.
echo ?? Para avaliar o sistema:
echo   1. Execute: INICIAR-AVALIACAO.bat
echo   2. Importe collections no Postman
echo   3. Execute testes automaticos
echo.
echo ?? Arquivos de limpeza:
echo   • Este script: LIMPAR-ARQUIVOS.bat
echo   • Documentacao: LIMPEZA-ARQUIVOS.md
echo.
pause