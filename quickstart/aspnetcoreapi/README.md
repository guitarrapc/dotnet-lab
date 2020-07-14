## TL;DR

ASP.NET Core API quick start app.

[Dockerfile](https://github.com/guitarrapc/dotnet-lab/blob/master/quickstart/aspnetcoreapi/aspnetcore-api-quickstart/Dockerfile)

## Image Tags

* `latest`: latest version of tag.
* `3.1`: ASP.NET Core API 3.1

## Run

Run latest.

```shell
docker run --rm -p 8088:80 guitarrapc/asptnetcoreapi-quickstart:latest
```

Run ASP.NET Core 3.1.

```shell
docker run --rm -p 8088:80 guitarrapc/asptnetcoreapi-quickstart:3.1
```

cURL request.

```shell
curl http://localhost:8088/healthz
curl http://localhost:8088/api/hello
curl http://localhost:8088/api/hello?name=hoge
curl http://localhost:8088/api/weatherforecast
```