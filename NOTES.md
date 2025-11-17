# Notes

## Installation

- `dotnet new tool-manifest`
- `dotnet tool install dotnet-reportgenerator-globaltool`

## Update

- `dotnet tool update dotnet-reportgenerator-globaltool`

## Tests

- Run:
  `dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:CoverletOutputFormat=cobertura /p:CoverletOutput='../coverage.cobertura.xml' /p:ExcludeByAttribute="GeneratedCodeAttribute" /p:ExcludeByFile="**/Program.cs"`
- Report: `dotnet reportgenerator -reports:./coverage.cobertura.xml -targetdir:./TestResults -reporttypes:Html`

In one Go!

```bash
dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:CoverletOutputFormat=cobertura /p:CoverletOutput='../coverage.cobertura.xml' /p:ExcludeByAttribute="GeneratedCodeAttribute" /p:ExcludeByFile="**/Program.cs"
dotnet reportgenerator -reports:./coverage.cobertura.xml -targetdir:./TestResults -reporttypes:Html
```

## Info

- https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/MSBuildIntegration.md
- https://github.com/danielpalme/ReportGenerator
