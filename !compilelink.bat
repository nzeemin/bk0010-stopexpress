@echo off

for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "DATESTAMP=%YYYY%;%MM%;%DD%"
for /f %%i in ('git rev-list HEAD --count') do (set REVISION=%%i)
echo REV%REVISION% %DATESTAMP%

echo 	.ASCII /REV%REVISION% %DATESTAMP%/ > VERSIO.MAC
echo 	.IDENT /V%REVISION%/ >> VERSIO.MAC

@if exist TILES.OBJ del TILES.OBJ
@if exist EXPRES.LST del EXPRES.LST
@if exist EXPRES.OBJ del EXPRES.OBJ

C:\bin\rt11\rt11.exe MACRO/LIST:DK: EXPRES.MAC

for /f "delims=" %%a in ('findstr /B "Errors detected" EXPRES.LST') do set "errdet=%%a"
if "%errdet%"=="Errors detected:  0" (
  echo COMPILED SUCCESSFULLY
) ELSE (
  findstr /RC:"^[ABDEILMNOPQRTUZ] " EXPRES.LST
  echo ======= %errdet% =======
  exit /b
)

@if exist OUTPUT.MAP del OUTPUT.MAP
@if exist EXPRES.SAV del EXPRES.SAV

C:\bin\rt11\rt11.exe LINK EXPRES /MAP:OUTPUT.MAP

for /f "delims=" %%a in ('findstr /B "Undefined globals" OUTPUT.MAP') do set "undefg=%%a"
if "%undefg%"=="" (
  type OUTPUT.MAP
  echo.
  echo LINKED SUCCESSFULLY
) ELSE (
  echo ======= LINK FAILED =======
  exit /b
)

if exist EXPRES.BIN del EXPRES.BIN
C:\bin\Sav2BkBin.exe EXPRES.SAV EXPRES.BIN
