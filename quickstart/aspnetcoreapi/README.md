```shell
docker run --rm -p 8088:80 guitarrapc/asptnetcoreapi-quickstart:3.1
curl http://localhost:8088/healthz
curl http://localhost:8088/api/hello
curl http://localhost:8088/api/hello?name=hoge
curl http://localhost:8088/api/weatherforecast
```