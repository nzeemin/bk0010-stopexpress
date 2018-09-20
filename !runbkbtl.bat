@echo off

if not exist EXPRES.BIN (
  echo.
  echo ####### EXPRES.BIN not found #######
  exit /b
)

cd x-bkbtl

start BKBTL.exe
