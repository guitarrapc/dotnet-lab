## TL;DR

dotnet restore、検証結果からも依存プロジェクトも再帰的にリストアは可能

これは 1.0 以降の by design となっている。(それ以前は、csprojの依存にかかわらず勝手にcsproj を探して restore してた)

> by design. we are now working similarly to "dotnet build". given a project, we will recursively restore all project dependencies
> 
> https://github.com/NuGet/Home/issues/4849

なお、もしも対象のcsproj を定めてまとめてリストアしたい場合は、sln をつくれとのこと。

## dotnet build は自動的にrestoreをする検証

```
>dotnet build
.NET Core 向け Microsoft (R) Build Engine バージョン 15.9.20+g88f5fadfbe
Copyright (C) Microsoft Corporation.All rights reserved.

  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj のパッケージを復元しています...
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj のパッケージを復元しています...
  Newtonsoft.Json 12.0.1 をインストールしています。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj の復元が 326.11 ms で完了しました。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj の復元が 326.11 ms で完了しました。
  ClassLibrary1 -> C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\bin\Debug\netstandard2.0\ClassLibrary1.dll
  ConsoleApp1 -> C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\bin\Debug\netcoreapp2.2\ConsoleApp1.dll

ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:01.07
```

## dotnet build --no-restore は restore をしない検証

Newtonsoft.Json の Cache がない状態で dotnet build --no-restoreを csproj にかける -> Newtonsoft.Json がないので落ちる

```
> dotnet build --no-restore
.NET Core 向け Microsoft (R) Build Engine バージョン 15.9.20+g88f5fadfbe
Copyright (C) Microsoft Corporation.All rights reserved.

C:\Program Files\dotnet\sdk\2.2.104\Microsoft.Common.CurrentVersion.targets(2110,5): warning MSB3106: アセンブリの厳密な名前 "C:\Users\guitarrapc\.nuget\packages\newtonsoft.json\12.0.1\lib\netstandard2.0\Newtonsoft.Json.dll" は、見つからな かったパスであるか、形式が正しくない完全アセンブリ名です。完全アセンブリ名の場合、円記号 (\) でエスケープされなければな らない文字を含んでいる可能性があります。それらの文字は、等号 (=)、コンマ (,)、二重引用符 (")、アポストロフィ (')、円記号 (\) です。 [C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj]
C:\Program Files\dotnet\sdk\2.2.104\Microsoft.Common.CurrentVersion.targets(2110,5): warning MSB3106: アセンブリの厳密な名前 "C:\Users\guitarrapc\.nuget\packages\newtonsoft.json\12.0.1\lib\netstandard2.0\Newtonsoft.Json.dll" は、見つからな かったパスであるか、形式が正しくない完全アセンブリ名です。完全アセンブリ名の場合、円記号 (\) でエスケープされなければな らない文字を含んでいる可能性があります。それらの文字は、等号 (=)、コンマ (,)、二重引用符 (")、アポストロフィ (')、円記号 (\) です。 [C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj]
Class1.cs(1,7): error CS0246: 型または名前空間の名前 'Newtonsoft' が見つかりませんでした (using ディレクティブまたはアセンブリ参照が指定されていることを確認してください)。 [C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj]

ビルドに失敗しました。

C:\Program Files\dotnet\sdk\2.2.104\Microsoft.Common.CurrentVersion.targets(2110,5): warning MSB3106: アセンブリの厳密な名前 "C:\Users\guitarrapc\.nuget\packages\newtonsoft.json\12.0.1\lib\netstandard2.0\Newtonsoft.Json.dll" は、見つからな かったパスであるか、形式が正しくない完全アセンブリ名です。完全アセンブリ名の場合、円記号 (\) でエスケープされなければな らない文字を含んでいる可能性があります。それらの文字は、等号 (=)、コンマ (,)、二重引用符 (")、アポストロフィ (')、円記号 (\) です。 [C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj]
C:\Program Files\dotnet\sdk\2.2.104\Microsoft.Common.CurrentVersion.targets(2110,5): warning MSB3106: アセンブリの厳密な名前 "C:\Users\guitarrapc\.nuget\packages\newtonsoft.json\12.0.1\lib\netstandard2.0\Newtonsoft.Json.dll" は、見つからな かったパスであるか、形式が正しくない完全アセンブリ名です。完全アセンブリ名の場合、円記号 (\) でエスケープされなければな らない文字を含んでいる可能性があります。それらの文字は、等号 (=)、コンマ (,)、二重引用符 (")、アポストロフィ (')、円記号 (\) です。 [C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj]
Class1.cs(1,7): error CS0246: 型または名前空間の名前 'Newtonsoft' が見つかりませんでした (using ディレクティブまたはアセンブリ参照が指定されていることを確認してください)。 [C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj]
    2 個の警告
    1 エラー

経過時間 00:00:00.62
```

## dotnet restore がプロジェクト参照した依存プロジェクトのnuget もリストアするのか検証

dotnet restore を csproj に対して実行する -> 依存関係のClassLibrary1.csproj の NuGet が復元される

```
dotnet restore
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj のパッケージを復元しています...
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj のパッケージを復元しています...
  Newtonsoft.Json 12.0.1 をインストールしています。
  MSBuild ファイル C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\obj\ClassLibrary1.csproj.nuget.g.props を 生成しています。
  MSBuild ファイル C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\obj\ConsoleApp1.csproj.nuget.g.props を生成 しています。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj の復元が 788.77 ms で完了しました。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj の復元が 788.77 ms で完了しました。
```

その後のビルドも成功する

```
>dotnet restore
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj のパッケージを復元しています...
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj のパッケージを復元しています...
  Newtonsoft.Json 12.0.1 をインストールしています。
  MSBuild ファイル C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\obj\ClassLibrary1.csproj.nuget.g.props を 生成しています。
  MSBuild ファイル C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\obj\ConsoleApp1.csproj.nuget.g.props を生成 しています。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj の復元が 788.77 ms で完了しました。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj の復元が 788.77 ms で完了しました。

C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1>dotnet build --no-restore
.NET Core 向け Microsoft (R) Build Engine バージョン 15.9.20+g88f5fadfbe
Copyright (C) Microsoft Corporation.All rights reserved.

  ClassLibrary1 -> C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\bin\Debug\netstandard2.0\ClassLibrary1.dll
  ConsoleApp1 -> C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\bin\Debug\netcoreapp2.2\ConsoleApp1.dll

ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:00.92
```

## dotnet restore がプロジェクト参照した依存プロジェクトAがさらに依存したプロジェクトBのnuget もリストアするのか検証

問題なくリストアする。

```
>dotnet restore
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj のパッケージを復元しています...
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj のパッケージを復元しています...
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary2\ClassLibrary2.csproj のパッケージを復元しています...
  Newtonsoft.Json 12.0.1 をインストールしています。
  MSBuild ファイル C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary2\obj\ClassLibrary2.csproj.nuget.g.props を 生成しています。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj の復元が 336.97 ms で完了しました。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary2\ClassLibrary2.csproj の復元が 336.99 ms で完了しました。
  C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\ClassLibrary1.csproj の復元が 336.97 ms で完了しました。
  
  >dotnet build --no-restore
.NET Core 向け Microsoft (R) Build Engine バージョン 15.9.20+g88f5fadfbe
Copyright (C) Microsoft Corporation.All rights reserved.

  ClassLibrary2 -> C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary2\bin\Debug\netstandard2.0\ClassLibrary2.dll
  ClassLibrary1 -> C:\Users\guitarrapc\source\repos\ConsoleApp1\ClassLibrary1\bin\Debug\netstandard2.0\ClassLibrary1.dll
  ConsoleApp1 -> C:\Users\guitarrapc\source\repos\ConsoleApp1\ConsoleApp1\bin\Debug\netcoreapp2.2\ConsoleApp1.dll

ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:00.85
```