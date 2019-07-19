## Migration について

* クラスを生成して、DBに追加したい。

migration を作って、適用すればok

```
dotnet ef migrations add マイグレーション名
dotnet ef database update マイグレーション名
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

ただし、`EnsureCreated()` は `Migrate()` の後に実行すること。(先にテーブルが作成されるため、Migration がこけるようになります)

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

Migration は、コードファーストなクラスと DBの完全同期はせず、Migration を当てるのみです。
そのため、DBに先にMigrationで行う変更をして、そこを触るコードを生成、Migrationを実行するとMigration がこけます。

* DB に先に変更してしまった。Migration が失敗する。

DB の変更を戻す or DBの対象テーブルなどをリネーム、Migration をあてる、リネームしたテーブルのデータを移すかMigrationで作ったテーブルを排除。

* DB の同期をしたい

docker などにMigration をあてて、対象のテーブルとスキーマ比較、実行。

MSSQL の場合、Visual Studio で SQLServer Database Project を作成して Sheme compare -> sync が強力で便利。

MySQL の場合、[MySQL Workbench]() を使って、[Synchronize with any source](https://dev.mysql.com/doc/workbench/en/wb-database-synchronization.html) を使うことで同期や差分のSQL 生成が可能。

* データの同期をしたい

通常のSQL実行やただの完全同期なら、[HeidiSQL のExport Sql](https://www.heidisql.com/screenshots.php?which=export_sql) を使うことでデータベースや選択したデータを一つのダンプに出力、テーブルごとに出力、直接対象DBに実行できます。ほぼ mysqldump と同じ速度で高速な処理が期待できるのが特徴です。(実行インスタンスのネットワーク帯域やメモリに依存する部分があります)

一方で、テーブルの作成は行ってもカラムのdiffをとって適用はしないので、ただの適用には向きません。一度テーブルをドロップして作成するならいいのですが、ドロップなしでのテーブルスキーマの差分Diff SQL生成はできないので、そういう用途はMySQL Workbench を使うほうがいいでしょう。

ツール | スキーマ同期 | スキーマDiff SQL生成 | データ同期 | 備考
---- | ---- | ---- | ----
MySQL Workbench | 〇 | 〇 | × | カラムのDiffなども出せる
HeidiSQL | 〇 | × | 〇 | テーブルのカラムDiffができない。スキーマ同期をする場合、一度dropしてcreateしてからデータ挿入になるため、ちがうそれじゃないケースが多々ある。

* MySQL のツーリングをWindows にインストールしたい

Windows で MySQL Workbench や HeidiSQL をインストールときは scoop が便利。

```
scoop install mysql-workbench
scoop install heidisql
```

