if dotnet tool list -g | grep unitybuildrunner;
then
    dotnet tool update --global UnityBuildRunner
else
    dotnet tool install --global UnityBuildRunner --version 1.1.5
fi
