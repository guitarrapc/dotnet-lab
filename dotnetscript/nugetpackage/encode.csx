#!/usr/bin/env dotnet-script
#r "nuget:Base64UrlCore,1.1.0"
using Base64UrlCore;

Console.WriteLine(Base64Url.Encode(Args[0]));
