Please download [dotnet](https://dotnet.microsoft.com/download) with the SDK. These steps have
been tested with .NET 5.0.

### Nuget sources

The  `nuget.config` in `LPSharp/LPSharp` points to three nuget sources that contain
solver packages.
- __LocalCLP__ is the CMake build directory of CLP (`CLP/build/dotnet/packages`).
- __LocalGLOP__ is the CMake build directory of GLOP (`GLOP/build/dotnet/packages`). 
- __LPSharp__ is the [Azure DevOps feed](https://msazure.visualstudio.com/One/_packaging?_a=feed&feed=LPSharp).

If you have not built the solvers or built them in a folder not named `build`, then
the first two sources will not work. Dotnet nuget command traverses up the directory
tree to create its source list.

Verify the sources.

```
$ cd LPSharp/LPDriver
$ dotnet nuget list source

Registered Sources:
  1.  LocalCLP [Enabled]
      C:\Users\krish\Work\LPSharp\CLP\build\dotnet\packages
  2.  LocalGLOP [Enabled]
      C:\Users\krish\Work\LPSharp\GLOP\build\dotnet\packages
  3.  LPSharp [Enabled]
      https://msazure.pkgs.visualstudio.com/One/_packaging/LPSharp/nuget/v3/index.json
  4.  nuget.org [Enabled]
      https://api.nuget.org/v3/index.json
```

### Verifying the solver packages used in dotnet build

Dotnet build restores packages referenced in the project file. Restoring means searching
nuget sources for the closest matching version of the package. Only the LPDriver project
references solver packages. Hence execute these statements in that folder.

View the package versions dotnet will restore with.

```
$ cd LPSharp/LPDriver
$ dotnet list package
Project 'LPDriver' has the following package references
   [net5.0]:
   Top-level Package          Requested        Resolved
   > CoinOr.Clp               1.17.1905.36     1.17.1905.36
   > Google.OrTools.Glop      9.1.9490.38      9.1.9490.38
   > StyleCop.Analyzers       1.2.0-beta.354   1.2.0-beta.354
```

To change the package to a new version, simply edit the `LPDriver.csproj` project file.

### Publishing solver packages to the LPSharp nuget feed

LPSharp nuget feed is hosted in Azure DevOps. The [feed](https://msazure.visualstudio.com/One/_packaging?_a=feed&feed=LPSharp)
has instructions to connect to it, and they are reproduced below.
This feed requires the Azure Credential Provider nuget plugin. Install the plugin
using the following command from a powershell window.

```
PS $ iex "& { $(irm https://aka.ms/install-artifacts-credprovider.ps1) } -AddNetfx"
```

Push packages to the feed.

```
$ cd LPSharp
$ dotnet nuget push --interactive --source LPSharp --api-key az ../CLP/build/dotnet/packages/CoinOr.Clp.<version>.nupkg
$ dotnet nuget push --interactive --source LPSharp --api-key az ../CLP/build/dotnet/packages/CoinOr.Clp.runtime.win-x64.<version>.nupkg

$ dotnet nuget push --interactive --source LPSharp --api-key az ../GLOP/build/dotnet/packages/Google.OrTools.Glop.<version>.nupkg
$ dotnet nuget push --interactive --source LPSharp --api-key az ../GLOP/build/dotnet/packages/Google.Ortools.Glop.runtime.win-x64.<version>.nupkg
```

Add the `--interactive` option if it is the first time. You will be shown a device login link
with a code to authenticate you with the feed. The `--api-key az` options causes
nuget to use the credential provider you just installed. If you get an `Unauthorized` error,
please check whether you installed the credential provider (check contents of `$HOME/.nuget/plugins`),
and specified the api-key and interactive options.