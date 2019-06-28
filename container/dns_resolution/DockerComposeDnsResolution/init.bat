dotnet publish DockerComposeDnsResolution -o bin/output -r linux-x64
dotnet publish WebApp -o bin/output -r linux-x64

docker-compose build
docker-compose up -d
docker-compose ps
docker-compose logs -f
