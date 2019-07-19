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

ないです。

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
---- | ---- | ---- | ---- | ----
MySQL Workbench | 〇 | 〇 | × | カラムのDiffなども出せる
HeidiSQL | 〇 | × | 〇 | テーブルのカラムDiffができない。スキーマ同期をする場合、一度dropしてcreateしてからデータ挿入になるため、ちがうそれじゃないケースが多々ある。

* MySQL のツーリングをWindows にインストールしたい

Windows で MySQL Workbench や HeidiSQL をインストールときは scoop が便利。

```
scoop install mysql-workbench
scoop install heidisql
```

* Migration snapshot がほかのCommitとconflict する

> ref: https://docs.microsoft.com/ja-jp/ef/core/managing-schemas/migrations/teams

snapshot は、現在のスキーマ定義を示すため、次のパターンが考えられます。

> 新しいプロパティの追加: 新しいプロパティは異なるエンティティであるためgit の自動マージで解消されるはずです。

例えば、チームで新しいプロパティを追加したpush をしていてpullしたとします。

次のようなDiffが出ますが、これは両方とも共存できるエンティティであるはずなので自動的に解消されます。

```
<<<<<<< Mine
b.Property<bool>("Deactivated");
=======
b.Property<int>("LoyaltyPoints");
>>>>>>> Theirs
```

```
b.Property<bool>("Deactivated");
b.Property<int>("LoyaltyPoints");
```

> 既存のプロパティを変更した: チームが自分の環境のプロパティをリネームした場合

このとき、自分のモデルのスナップショットとチームのスナップショットで競合が起きます。

```
<<<<<<< Mine
b.Property<string>("Username");
=======
b.Property<string>("Alias");
>>>>>>> Theirs
```

この場合、次のステップでマージ解消を目指します。

手元のマージをロールバックしてマージ前の状態に戻し、自分のモデルはそのままにマイグレーションだけ消し、チームの変更をマージしてから、自分の環境でマイグレーションを生成します。

## MySQL 特有の注意

* clr と mysql の型マッピング

次のエンティティを考えます。

```csharp
    public class TestType
    {
        [Key]
        public int Id { get; set; }
        public sbyte Sbyte { get; set; }
        public byte Byte { get; set; }
        public short Short { get; set; }
        public ushort Ushort { get; set; }
        public int Int { get; set; }
        public uint Uint { get; set; }
        public long Long { get; set; }
        public ulong Ulong { get; set; }
        public float Float { get; set; }
        public double Double { get; set; }
        public bool Bool { get; set; }
        public string String { get; set; }
        public DateTime Datetime { get; set; }
        public DateTimeOffset DatetimeOffset { get; set; }
    }
```

これを entity framework で migration すると、MySQL で次の型で扱われます。

```sql
CREATE TABLE `TestType` (
	`Id` INT(11) NOT NULL AUTO_INCREMENT,
	`Sbyte` SMALLINT(6) NOT NULL,
	`Byte` TINYINT(4) NOT NULL,
	`Short` SMALLINT(6) NOT NULL,
	`Ushort` INT(11) NOT NULL,
	`Int` INT(11) NOT NULL,
	`Uint` BIGINT(20) NOT NULL,
	`Long` BIGINT(20) NOT NULL,
	`Float` FLOAT NOT NULL,
	`Double` DOUBLE NOT NULL,
	`Bool` BIT(1) NOT NULL,
	`String` TEXT NULL,
	`Datetime` DATETIME NOT NULL,
	`DatetimeOffset` TIMESTAMP NOT NULL,
	PRIMARY KEY (`Id`)
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
```

* TypeConversion のずれ

以下の Entity Framework の型変換が注意を要します。

clr | MySQL | 型変換指定
---- | ---- | ----
string | text | data annotation OR fluentAPI
DatetimeOffset | TimeStamp | TypeConversion
bool | tinyint(4) | data annotation OR fluentAPI
unsigned | signed にマップ | Entity Framework としてサポートしていない....

型変換を明示的に行うには、data annotation OR fluentAPI と TypeConversion の2方法があります。

> https://docs.microsoft.com/ja-jp/ef/core/modeling/max-length

data annotationは、プロパティにAttributeでアノテーションを指示するものです。

```csharp
[Column(TypeName = "VARCHAR(255)")]
public string String2 { get; set; }
```

fluentAPI は、DbContext の `OnModelCreating` を override して指示をします。

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<TypeTable>()
        .Property(e => e.String2)
        .HasColumnType("VARCHAR(255)");
}
```

unsigned は、ADO.NET の SQL Server でサポートされていないために、Entity Framework のアノテーションにはありません。
コードファーストで生成したテーブルのカラムでも指定できません....

DB上は long にいれておいて、byte にいれてから ulong にキャストする方法がありますが.....

> [mysql \- How to use unsigned int / long types with Entity Framework? \- Stack Overflow](https://stackoverflow.com/questions/26303631/how-to-use-unsigned-int-long-types-with-entity-framework/26436056)


* クエリが SQL Server 向けにはかれる

例えば、Entity に typename ではなく、MaxLength Attirbute で長さを指定するとこの問題に遭遇する。

```csharp
[Column(TypeName = "VARCHAR")]
[MaxLength(500)]
public string String2 {get; set;}
```

このMigrationを実行すると、mysql を providerに指定していても、Alter table が sql server 向けのSQLが実行されて実行時エラーになる。

```sql
ALTER TABLE TestType MODIFY `String2` VARCHAR NOT NULL;
```

mysql ならこの構文になる

```sql
ALTER TABLE `TestType` CHANGE COLUMN `String2` `String2` VARCHAR(50) NULL AFTER `DatetimeOffset`;
```

回避するには、Entity での指定を、`[Column(TypeName = "VARCHAR(50)")]` にする。
これなら生成されるクエリはMySQL を対象になっており、`[Column(TypeName = "VARCHAR(255)")]`にしても適用できる。

なお、この問題はVARCHAR での指定では発生するが、byte[] と VARBINARY間では発生しない。

```csharp
// 問題ない
[MaxLength(3000)]
public byte[] ByteArray { get; set; }
```

fluentAPI であってもこれは同様にStringの場合、HasMaxLength で縛るとエラーが生じます。

```csharp
// do
modelBuilder
    .Entity<TestType>()
    .Property(e => e.String3)
    .HasColumnType("VARCHAR(255)");

// do not
modelBuilder
    .Entity<TestType>()
    .Property(e => e.String3)
    .HasColumnType("VARCHAR")
    .HasMaxLength(255);

// error
// Failed executing DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
// ALTER TABLE `TestType` ADD `String3` VARCHAR NULL;
// You have an error in your SQL syntax; check the manual that corresponds to your MySQL server version for the right syntax to use near 'NULL' at line 1
```
