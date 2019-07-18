## Migration について

* クラスを生成して、DBに追加したい。

migration を作って、適用すればok

```
dotnet ef migrations add マイグレーション名
```

* Migration で実行される sql を適用前に生成したい

from (適用された最後のmigration) と to (SQLを生成したい Migration名) を指定すると生成される。

```
dotnet ef migrations script FROM TO
```

* どの Migration まで適用されたか確認したい

`__EFMigrationsHistory` Table に履歴があるので閣員できる。

ここにあるのは「適用された」Migrate なので、ないということはまだない。
revert するとこのテーブルからmigrate が消える。

* 適用したMigration を revert したい

戻したい時点のMigration名を指定すればok

```
dotnet ef database update 戻したい時点のマイグレーション名
```

例: InitialCreate をあててから、AddTable を当てたが、AddTable を revert したい。(InitialCreate 時点に戻したい)

```
dotnet ef database update InitialCreate
```

* プログラムから Migration を適用したい

DbContext の Database.Migrate() を実行。

```csharp
using (var db = new MyDbContext())
{
    db.Database.Migrate();
}
```

ただし、`EnsureCreated()` は Migrate() の後に実行すること。(Migrate がこけるようになります)

* migrate をかける前に直接DBに変更してしまったがどうすればいいのか

DB に行った変更を戻す必要があります。
強制同期がない..... ので、mysql workbench などで 開発環境と対象DBを同期させたり、まとめてMigration をかける必要があります。

* migaration で使う DBContext をとりたい

取れます。

```
dotnet ef dbcontext info
```

* migrationを強制的にスキップしたい

[TODO]

## 注意しないといけない点

Migration は、コードファーストなクラスと DBの完全同期はせず、Migrateion を当てるのみです。
そのため、DBに先にMigrationで行う変更をして、そこを触るコードを生成、Migrationを実行するとMigration がこけます。

* DB に先に変更してしまった。Migration がしっぱいする。

DB の変更を戻す or DBの対象テーブルなどをリネーム、Migration をあてる、リネームしたテーブルのデータを移すかMigrationで作ったテーブルを排除。

