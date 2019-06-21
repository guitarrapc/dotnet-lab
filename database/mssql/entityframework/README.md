## Entity Framework Core with Windows and Docker Container

Run EF core on both Windows and Docker Linux(stretch).

This is Proof of Concept that Dev on Windows and run on Container.

## How to run

open cmd/powershell/terminal and run docker-compose.

```
docker-compose up -d
```

stop container when you want to quit.

```
docker-compose down
```

check container status logs.

```
docker-compose logs -f
```

if any trouble, container will be failed to launch.

```
docker-compose ps
```

build web container image if you want renew app.

```
docker-compose build
```
