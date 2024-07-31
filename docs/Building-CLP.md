The examples below assume that the current directory is the root of CLP sources,
and your desired build directory is `./build`. You can download CMake from
[here](https://cmake.org/download/).

Generate the CMake build system, fetch and build dependent packages.

```
cmake -S . -B build
```

You can browse the code using `build\coinor.sln`.

Build the release.

```
cmake --build build --config Release
```

You should find the following artifacts upon a successful build:

- `build/dotnet/packages/CoinOr.Clp.<version>.nupkg`
- `build/dotnet/packages/CoinOr.Clp.runtime.win-x64.<version>.nupkg`
- `build/CoinWrap/cpp/Release/coinwrap.exe` (CoinWrap standalone executable)
- `build/Clp/src/Release/Clp.exe` (CLP standalone executable)

The native library has Clp interface, libClp, and libCoinUtils. The .Net library
has the C# code. The version number is based on the public release number and
the commit counts in the Clp and CoinWrap code bases.

Import the packages into LPSharp or your C# project using:

```
dotnet add package CoinOr.Clp --version <version number>
```