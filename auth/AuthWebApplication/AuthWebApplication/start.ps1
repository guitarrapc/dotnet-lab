docker-compose down
docker-compose up -d
Start-Sleep -Seconds 5
dotnet ef database update
docker-compose ps