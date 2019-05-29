## Rin the Glimpse alternative in .NET Core

> https://github.com/mayuki/Rin

## Access Control

> https://github.com/mayuki/Rin/issues/11

```
var options = app.ApplicationServices.GetService<RinOptions>();
app.MapWhen(
    ctx => ctx.Request.Path.StartsWithSegments(options.Inspector.MountPath) &&
           /* ctx.Request ...some conditions ... */,
    app2 =>
    {
        app2.Use((ctx, next) =>
        {
            ctx.Response.StatusCode = 403;
            ctx.Response.WriteAsync("Forbidden");
            return Task.CompletedTask;
        });
    });

app.UseRin();

// your code....
```
