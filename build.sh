TYPE=Release
NUGET_FOLDER=$USERPROFILE/.nuget/packages
XUNIT_VERSION=2.4.1
XUNIT_CONSOLE=$NUGET_FOLDER/xunit.runner.console/$XUNIT_VERSION/tools/net452/xunit.console.exe

dotnet restore src
msbuild.exe "$PWD/src/Caliburn.sln" //p:Configuration=$TYPE
if [[ $? != 0 ]]; then exit 1; fi

$XUNIT_CONSOLE src/Tests.Caliburn/bin/$TYPE/net452/Tests.Caliburn.dll
if [[ $? != 0 ]]; then exit 1; fi

dotnet pack src/Caliburn/Caliburn.csproj --configuration $TYPE --output $PWD/artifacts --no-restore
dotnet pack src/Caliburn.Testability/Caliburn.Testability.csproj --configuration $TYPE --output $PWD/artifacts --no-restore
