run followings at WebApplicationEF directory to init db class.

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
