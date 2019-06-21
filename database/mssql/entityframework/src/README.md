## step to prepare model/data/controller/view

### choice 1. run followings at WebApplicationEF directory to init db class.

use migration to create table on database. (Model first)

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update --configuration appsettings.json
```

create controller

```
dotnet aspnet-codegenerator controller -name BlogsController -m Blog -dc BloggingContext --relativeFolderPath Controllers --useDefaultLayout
dotnet aspnet-codegenerator controller -name PostsController -m Post -dc BloggingContext --relativeFolderPath Controllers --useDefaultLayout
dotnet aspnet-codegenerator controller -name UsersController -m User -dc BloggingContext --relativeFolderPath Controllers --useDefaultLayout
```

### choice 2. manage Database .sql with DatabaseEF

use sql to create table on database. (DB first)

* Connect to database and update sql.

## Optimization

* DO: Use ConnectionPooling with `AddDbContextPool`, do not use simple `AddDbContext` as it creates new instance of the DbContext for each request.
    * Good: `services.AddDbContextPool<BloggingContext>(optionBuilder => optionBuilder.UseSqlServer(connectionBuilder.ConnectionString));`
    * Bad:  `services.AddDbContext<BloggingContext>(options => options.UseSqlServer(connection));` // Startup.cs
        * no pooling results, 5-10min to kill sqlserver as of over pressure on memory.
