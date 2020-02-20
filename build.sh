TYPE=Release

dotnet restore src
dotnet build src -c $TYPE
if [[ $? != 0 ]]; then exit 1; fi

dotnet test src -c $TYPE
if [[ $? != 0 ]]; then exit 1; fi

dotnet pack src/Caliburn/Caliburn.csproj --configuration $TYPE --output $PWD/artifacts --no-restore
dotnet pack src/Caliburn.Testability/Caliburn.Testability.csproj --configuration $TYPE --output $PWD/artifacts --no-restore
