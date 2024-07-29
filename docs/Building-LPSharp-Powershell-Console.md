This step requires [dotnet](https://dotnet.microsoft.com/download) or Visual Studio. Please
refer to the [dotnet cheat sheet](Dotnet-cheatsheet.md) for troubleshooting tips.

Build the LPSharp Powershell console.

```
$ cd LPSharp/PowershellConsole
$ dotnet build -c Release
```

Typical output.
```
Microsoft (R) Build Engine version 16.11.0+0538acc04 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored C:\Users\krish\Work\LPSharp\LPSharp\LPDriver\LPDriver.csproj (in 262 ms).
  Restored C:\Users\krish\Work\LPSharp\LPSharp\Powershell\Powershell.csproj (in 293 ms).
  Restored C:\Users\krish\Work\LPSharp\LPSharp\PowershellConsole\PowershellConsole.csproj (in 487 ms).
  LPDriver -> C:\Users\krish\Work\LPSharp\LPSharp\LPDriver\bin\release\net5.0\LPDriver.dll
  Powershell -> C:\Users\krish\Work\LPSharp\LPSharp\Powershell\bin\release\net5.0\Powershell.dll
  PowershellConsole -> C:\Users\krish\Work\LPSharp\LPSharp\PowershellConsole\bin\release\net5.0\PowershellConsole.dll

Build succeeded.
```

You should the following artifacts upon a successful build.

- `PowershellConsole/bin/Release/net5.0/PowershellConsole.exe` is the LPSharp powershell console.
  Execute it to access the cmdlets.
