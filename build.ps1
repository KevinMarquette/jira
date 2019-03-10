push-location .\src
dotnet clean
dotnet publish
Pop-Location

Import-module .\src\bin\Debug\netstandard2.0\publish\jiraModule.dll
