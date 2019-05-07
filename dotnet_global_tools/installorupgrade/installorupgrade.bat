dotnet tool list -g | findstr unitybuildrunner
if '%errorlevel%' EQU '0' (
    dotnet tool update --global UnityBuildRunner
) else (
    dotnet tool install --global UnityBuildRunner --version 1.1.7
)