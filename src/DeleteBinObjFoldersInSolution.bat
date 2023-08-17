@echo off

REM THIS FILE MUST BE STORED IN THE ROOT FOLDER OF THE SOLUTION
REM To run this file you execute it from your file explorer

@echo Deleting all BIN and OBJ folders
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"
@echo BIN and OBJ folders successfully deleted. Close the window.
pause > nul
