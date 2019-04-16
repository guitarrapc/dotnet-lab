dotnet tool list -g | findstr unitybuildrunner
if ('%errorlevel%' == '1') (
    dotnet tool install --global UnityBuildRunner --version 1.1.5
) else (
    dotnet tool update --global UnityBuildRunner
)
