## Entity Framework Core with Windows and Docker Container

Run EF core on both Windows and Docker Linux(stretch).

This is Proof of Concept that Dev on Windows and run on Container.

## How to run

re-build docker image.

```
docker-compose build --pull
```

open cmd/powershell/terminal and run docker-compose.

```
docker-compose up -d
```

check container up status and port mapping. if any trouble, container will be failed to launch.

```
docker-compose ps
```

access to the content, which runs ef.
make sure `docker-compose ps` shows `443/tcp, 0.0.0.0:5430->80/tcp` ports bindings.

```
# cmd/terminal
curl http://localhost:8080/Blogs

# powershell
curl.exe http://localhost:8080/Blogs
```

## scale out app server

comment out nginx lb settings to listen 5 backend.

```
    server entityframework_web_ef_2:80 weight=5 max_fails=3 fail_timeout=30s;
    server entityframework_web_ef_3:80 weight=5 max_fails=3 fail_timeout=30s;
    server entityframework_web_ef_4:80 weight=5 max_fails=3 fail_timeout=30s;
    server entityframework_web_ef_5:80 weight=5 max_fails=3 fail_timeout=30s;
```

scale web container to 5.

```
docker-compose up -d --scale web_ef=5
```

now you can access to 5 containers.
you can check nginx load balancing to asp.net core mvc via reloading http://localhost:8080

```
curl http://localhost:8080/Blogs
```

stop container when you want to quit.

```
docker-compose down
```

check container status logs.

```
docker-compose logs -f
```

build web container image if you want renew app.

```
docker-compose build
```
