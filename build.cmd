@echo off

pushd %~dp0

md artifacts
call "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" src\Caliburn.sln /p:Confiuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%
call dotnet test src/Tests.Caliburn
if %errorlevel% neq 0 exit /b %errorlevel%

echo Packing Caliburn Nuget
call dotnet pack src/Caliburn --configuration Release --output artifacts
call dotnet pack src/Caliburn.Testability --configuration Release --output artifacts

popd