## SourceLink

Source Link reference test project.

* Package: Json.Net 12.0+

> http://james.newtonking.com/archive/2018/11/27/json-net-12-0-release-1-net-foundation-nuget-and-authenticode-signing-sourcelink-and-more

## Step to sourcelink

* Enable Symbol Server
    * settings > Debugging > Symbols > Symbol file locations > Symbol file locations > **enable NuGet.org Symbol Server**
* Set Debugger on line 11
* Debug Run (F5)
* When Stopped on debugger, press F11 to step-in.
* Symbol download prompt will show, donwload.
* SourceLink will automatically download source code from Github and F11 step-in to actual source code inside Visual Studio.

when I check with Json.net 12.0.1, symbol will downlod to following path.

> Symbol will download to %LOCALAPPDATA%\SourceServer\b55f292f84618991301f84329082691510c23078f77b86da558e3f058eacb16a\Src\Newtonsoft.Json

## Notice

* Visual Studio with F12 won't link to source. (VS2017/2019)
* LinqPad won't work for sourcelink.
* No CodeLens for step-in symbol SourceCode.
