This step requires [dotnet](https://dotnet.microsoft.com/download) or Visual Studio. Please
refer to the [dotnet cheat sheet](Dotnet-cheatsheet.md) for troubleshooting tips.

Build the LPSharp Powershell console.

```
cd LPSharp/PowershellConsole
dotnet build -c Release
```

You should the following artifacts upon a successful build.

- `PowershellConsole/bin/Release/net6.0/LPSharp.exe` is the LPSharp powershell console.
  Execute it to access the cmdlets.
