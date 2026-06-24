@echo off
setlocal enabledelayedexpansion
cd /d "%~dp0"

echo.
echo ========================================================
echo   PromeRotation ACR Init Template - Setup Tool
echo ========================================================
echo.
echo This tool will transform the Init template into your ACR.
echo.

:: ============================================================
:: 1. Collect user input
:: ============================================================
:GET_NAME
set "ACR_NAME="
set /p ACR_NAME="ACR name (e.g. Bard, Whm, Smn, PascalCase): "
if "!ACR_NAME!"=="" (echo [ERROR] ACR name required! & goto GET_NAME)

:GET_AUTHOR
set "AUTHOR="
set /p AUTHOR="Author name (English, used for output dir): "
if "!AUTHOR!"=="" (echo [ERROR] Author name required! & goto GET_AUTHOR)

:GET_DISPLAY
set "DISPLAY="
set /p DISPLAY="Display name (Chinese OK, or press Enter to use Author): "
if "!DISPLAY!"=="" set "DISPLAY=!AUTHOR!"

:GET_VERSION
set "VERSION="
set /p VERSION="Version (e.g. 1.0.0.0, press Enter for default): "
if "!VERSION!"=="" set "VERSION=1.0.0.0"

:: ============================================================
:: 2. Job selection
:: ============================================================
echo.
echo ========================================================
echo   Select Job (enter number)
echo ========================================================
echo.
echo   --- TANK ---
echo    1. PLD  (19)    2. WAR  (21)
echo    3. DRK  (32)    4. GNB  (37)
echo   --- HEALER ---
echo    5. WHM  (24)    6. SCH  (28)
echo    7. AST  (33)    8. SGE  (40)
echo   --- MELEE ---
echo    9. MNK  (20)   10. DRG  (22)
echo   11. NIN  (30)   12. SAM  (34)
echo   13. RPR  (39)   14. VPR  (41)
echo   --- RANGED ---
echo   15. BRD  (23)   16. MCH  (31)
echo   17. DNC  (38)
echo   --- CASTER ---
echo   18. BLM  (25)   19. SMN  (27)
echo   20. RDM  (35)   21. PCT  (42)
echo.

:GET_JOB
set "JOB_INDEX="
set /p JOB_INDEX="Enter number (1-21): "
if "!JOB_INDEX!"=="" goto GET_JOB

set "JOB_ENUM=MCH" & set "JOB_ID=31"
if "!JOB_INDEX!"=="1"  (set "JOB_ENUM=PLD" & set "JOB_ID=19")
if "!JOB_INDEX!"=="2"  (set "JOB_ENUM=WAR" & set "JOB_ID=21")
if "!JOB_INDEX!"=="3"  (set "JOB_ENUM=DRK" & set "JOB_ID=32")
if "!JOB_INDEX!"=="4"  (set "JOB_ENUM=GNB" & set "JOB_ID=37")
if "!JOB_INDEX!"=="5"  (set "JOB_ENUM=WHM" & set "JOB_ID=24")
if "!JOB_INDEX!"=="6"  (set "JOB_ENUM=SCH" & set "JOB_ID=28")
if "!JOB_INDEX!"=="7"  (set "JOB_ENUM=AST" & set "JOB_ID=33")
if "!JOB_INDEX!"=="8"  (set "JOB_ENUM=SGE" & set "JOB_ID=40")
if "!JOB_INDEX!"=="9"  (set "JOB_ENUM=MNK" & set "JOB_ID=20")
if "!JOB_INDEX!"=="10" (set "JOB_ENUM=DRG" & set "JOB_ID=22")
if "!JOB_INDEX!"=="11" (set "JOB_ENUM=NIN" & set "JOB_ID=30")
if "!JOB_INDEX!"=="12" (set "JOB_ENUM=SAM" & set "JOB_ID=34")
if "!JOB_INDEX!"=="13" (set "JOB_ENUM=RPR" & set "JOB_ID=39")
if "!JOB_INDEX!"=="14" (set "JOB_ENUM=VPR" & set "JOB_ID=41")
if "!JOB_INDEX!"=="15" (set "JOB_ENUM=BRD" & set "JOB_ID=23")
if "!JOB_INDEX!"=="16" (set "JOB_ENUM=MCH" & set "JOB_ID=31")
if "!JOB_INDEX!"=="17" (set "JOB_ENUM=DNC" & set "JOB_ID=38")
if "!JOB_INDEX!"=="18" (set "JOB_ENUM=BLM" & set "JOB_ID=25")
if "!JOB_INDEX!"=="19" (set "JOB_ENUM=SMN" & set "JOB_ID=27")
if "!JOB_INDEX!"=="20" (set "JOB_ENUM=RDM" & set "JOB_ID=35")
if "!JOB_INDEX!"=="21" (set "JOB_ENUM=PCT" & set "JOB_ID=42")

if "!JOB_ENUM!"=="MCH" if not "!JOB_INDEX!"=="16" (
    echo [ERROR] Invalid number! & goto GET_JOB
)

:: ============================================================
:: 3. Confirm
:: ============================================================
echo.
echo ========================================================
echo   Please confirm:
echo ========================================================
echo.
echo   ACR Name : !ACR_NAME!
echo   Display  : !DISPLAY!
echo   Author   : !AUTHOR!
echo   Version  : !VERSION!
echo   Job      : !JOB_ENUM! (ID=!JOB_ID!)
echo.
set "CONFIRM="
set /p CONFIRM="Proceed? (Y/n): "
if /i not "!CONFIRM!"=="Y" if /i not "!CONFIRM!"=="" (echo Cancelled. & goto :EOF)

:: ============================================================
:: 4. Set env vars and run PowerShell script
:: ============================================================
echo.
echo [INFO] Running setup...

set "_PS_ACR=!ACR_NAME!"
set "_PS_DISPLAY=!DISPLAY!"
set "_PS_AUTHOR=!AUTHOR!"
set "_PS_VER=!VERSION!"
set "_PS_JOB=!JOB_ENUM!"
set "_PS_JOBID=!JOB_ID!"
set "_PS_DIR=%~dp0Init"

set "PSFILE=%~dp0_setup_core.ps1"

PowerShell -NoProfile -ExecutionPolicy Bypass -File "%PSFILE%"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Setup failed! See output above.
    pause
    goto :EOF
)

:: Delete helper files
del "%PSFILE%" 2>nul
del "%~f0" 2>nul

echo.
echo [DONE] Template initialized successfully!
echo.
pause
