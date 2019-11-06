@echo off
set rt11exe=C:\bin\rt11\rt11.exe
set pclink11=C:\bin\pclink11.exe

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

%rt11exe% MACRO/LIST:DK: EXPRES.MAC

for /f "delims=" %%a in ('findstr /B "Errors detected" EXPRES.LST') do set "errdet=%%a"
if "%errdet%"=="Errors detected:  0" (
  echo COMPILED SUCCESSFULLY
) ELSE (
  findstr /RC:"^[ABDEILMNOPQRTUZ] " EXPRES.LST
  echo ======= %errdet% =======
  exit /b
)

@if exist EXPRES.MAP del EXPRES.MAP
@if exist EXPRES.SAV del EXPRES.SAV
@if exist EXPRES-11.MAP del EXPRES-11.MAP
@if exist EXPRES-11.SAV del EXPRES-11.SAV

%rt11exe% LINK EXPRES /MAP:EXPRES.MAP
@if exist EXPRES.MAP rename EXPRES.MAP EXPRES-11.MAP
@if exist EXPRES.SAV rename EXPRES.SAV EXPRES-11.SAV

%pclink11% EXPRES.OBJ /MAP > pclink11.log

fc.exe /b EXPRES-11.SAV EXPRES.SAV > fc.log
for /f "delims=" %%a in ('findstr /B "FC: " fc.log') do set "fcdiff=%%a"
if "%fcdiff%"=="FC: no differences encountered" (
  echo SAV FILES ARE EQUAL
  del fc.log
  echo.
) ELSE (
  echo ======= SAV FILES ARE DIFFERENT, see fc.log =======
  exit /b
)

for /f "delims=" %%a in ('findstr /B "Undefined globals" EXPRES.MAP') do set "undefg=%%a"
if "%undefg%"=="" (
  type EXPRES.MAP
  echo.
  echo LINKED SUCCESSFULLY
  echo.
) ELSE (
  echo ======= LINK FAILED =======
  exit /b
)

if exist EXPRES.BIN del EXPRES.BIN
C:\bin\Sav2BkBin.exe EXPRES.SAV EXPRES.BIN
