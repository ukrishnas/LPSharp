Please download [dotnet](https://dotnet.microsoft.com/download) with the SDK. These steps have
been tested with .NET 6.0.

### Nuget sources

The  `nuget.config` in `LPSharp/LPSharp` points to nuget sources that contain
solver packages.
- __LocalCLP__ is the CMake build directory of CLP (`CLP/build/dotnet/packages`).
- __LocalGLOP__ is the CMake build directory of GLOP (`GLOP/build/dotnet/packages`). 

If you have not built the solvers or built them in a folder not named `build`, then
the first two sources will not work. Dotnet nuget command traverses up the directory
tree to create its source list.

Verify the sources.

```
cd LPSharp/LPDriver
dotnet nuget list source
```

Output.
```
Registered Sources:
  1.  LocalCLP [Enabled]
      E:\GitHub\lpsharp\CLP\build\dotnet\packages
  2.  LocalGLOP [Enabled]
      E:\GitHub\lpsharp\GLOP\build\dotnet\packages
  3.  nuget.org [Enabled]
      https://api.nuget.org/v3/index.json
```

### Verifying the solver packages used in dotnet build

Dotnet build restores packages referenced in the project file. Restoring means searching
nuget sources for the closest matching version of the package. Only the LPDriver project
references solver packages. Hence execute these statements in that folder.

View the package versions dotnet will restore with.

```
cd LPSharp/LPDriver
dotnet list package
```

Output.
```
Project 'LPDriver' has the following package references
   [net6.0]:
   Top-level Package          Requested        Resolved
   > CoinOr.Clp               1.17.1905.186    1.17.1905.186
   > Google.OrTools.Glop      9.1.9490.186     9.1.9490.186
   > StyleCop.Analyzers       1.2.0-beta.354   1.2.0-beta.354
```

To change the package to a new version, simply edit the `LPDriver.csproj` project file.
