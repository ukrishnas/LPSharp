

The examples below assume that the current directory is the root of GLOP
sources, and your desired build directory is `./build`. 

Generate the build system, fetch and build dependent packages. No options are
required.

```
$ cd GLOP
$ cmake -S . -B build
```

You can browse the code using `build\ortools.sln`.

Build the release.

```
$ cmake --build build --config Release
```

You should find the following artifacts upon a successful build:

- `build/dotnet/packages/Google.OrTools.Glop.<version>.nupkg`
- `build/dotnet/packages/Google.OrTools.Glop.runtime.win-x64.<version>.nupkg`
- `build/RELEASE/bin/glop_solve.exe`

Import the packages into LPSharp or your C# project using:
```
$ dotnet add package Google.OrTools.Glop --version <version number>
```