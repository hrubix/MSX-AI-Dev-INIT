:: TestGame — MSXgl build (alternative layout: MSXgl/ inside this project)
@echo off
setlocal
if not defined MSXGL_PATH set MSXGL_PATH=MSXgl

:: Prefer MSXgl's SDCC; Chocolatey (and others) may ship a GCC cc1.exe that breaks SDCC.
set "PATH=%CD%\%MSXGL_PATH%\tools\sdcc\bin;%PATH%"

cls
"%MSXGL_PATH%\tools\build\Node\node.exe" "%MSXGL_PATH%\engine\script\js\build.js" %*
