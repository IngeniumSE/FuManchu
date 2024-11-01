@echo off

set target=%1
set verbostity=%2
if "%target%" == "" set target=Default
if "%verbostity%" == "" set verbostity=Minimal

rem Install dotnet tools
cd build
dotnet tool restore

rem Run our build app
cd apps\build
dotnet run --target %target% --verbostity %verbostity%
