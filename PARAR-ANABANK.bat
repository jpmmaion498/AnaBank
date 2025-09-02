@echo off
echo ?? AnaBank - Parando Sistema

echo.
echo ===============================================
echo    ANABANK - FINALIZANDO SISTEMA
echo ===============================================
echo.

echo ?? Parando todos os containers...
docker-compose -f docker-compose.production.yml down

echo.
echo ?? Limpando recursos (opcional)...
echo Deseja remover imagens e volumes? (S/N):
choice /c SN /n
if errorlevel 2 goto :skip_cleanup

echo   Removendo imagens...
docker-compose -f docker-compose.production.yml down --rmi all --volumes

echo   Removendo volumes orfaos...
docker volume prune -f

:skip_cleanup

echo.
echo ===============================================
echo     ? SISTEMA FINALIZADO COM SUCESSO!
echo ===============================================
echo.
echo ?? Para iniciar novamente: INICIAR-ANABANK.bat
echo.
pause