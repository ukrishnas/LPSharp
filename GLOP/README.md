# OR-Tools LP Solver

This folder contains the code for Google Operations Research Tools (OR-Tools)
linear programming (GLOP) solver.

## Change summary

Owing to the large and monolithic nature of the
[OR-Tools](https://github.com/google/ortools) public repository, the files in
this folder are a copy and not a git submodule. Many code modules have been
removed, and some have been changed. A helpful python script automates the copy
and patch steps. GLOP solver code depends on
[Abseil](https://github.com/abseil/abseil-cpp.git), [Protocol
Buffers](https://github.com/protocolbuffers/protobuf.git), and
[ZLib](https://github.com/madler/ZLIB.git). These are imported from their public
repositories during build.

__What has been retained?__

- GLOP solver code
- CMake build system
- SWIG support for C# language
- Platform support for dotnet on Windows x64

__What has been changed?__

- Build system changes:
  - The top-level CMakeLists.txt has been simplified and default values changed
    for a Windows dotnet build.
  - Only the needed cmake modules have been retained in the cmake/ folder and
    they have been made simpler.
  - Dependencies/CMakeLists.txt is used fetch the dependencies, abseil-cpp,
    protobuf, swig, and zlib from their git repositories and patched using
    patches in patches/.
  - Deps.cmake checks that the dependencies are present, and fails the build if
    they are not.
  - Utils.cmake has two functions to generate the project version. We use a
    4-number version, major.minor.patch.tweak, to differentiate our build from
    the public nuget which does not use tweak. Major and minor numbers are from
    Version.txt, patch is the count of commits in the branch, and tweak is
    always 1.
  - Cpp.cmake builds and installs the C++ sources. This in turn uses
    CMakeLists.txt in each ortools/ subdirectory: base, glop, linear_solver,
    lp_data, port, and util.
  - Dotnet.cmake generates C# sources using Swig code in csharp/ folders in
    linear_sharp and util subdirectories. It generates dotnet build files using
    input files in ortools/dotnet. In the build step, it produces two nuget
    packages, Google.OrTools.version.nupkg and
    Google.OrTools.runtime.win-x64.version.nupkg.
- The intent of the following changes is to fix code that references non-LP
  modules, like graph and algorithms, that are not part of the tree. 
  - Graph/iterators.h has been moved to util/ and the includes of this header
    file changed to the new location. This was originally meant for graph
    iteration but its usage has leaked into sparse_vector.h and a few other
    files.
  - Lp_data/lp_decomposer.{cc,h} files, which define the LPDecomposer type, were
    deleted. LPDecomposer is only used by binary optimization integer solver
    which has been removed. It references dynamic partitioning algorithm
    which is not part of the tree.

__What has been removed?__

- Documentation and examples:
  - All documentation (doc[s]/ folders)
  - All examples (examples/ folder)
  - All sample code (samples/ folders)
- Functionality:
  - Interfaces for third-party solvers (COIN, GLPK, Gurobi, SCIP, Xpress)
  - Boolean optimization solver
  - Constraint programming solver
  - Code for combinatorial optimization and graph algorithms
  - Code for vehicle routing
- Language support:
  - [Flat Zinc](https://www.minizinc.org/) modeling language
  - Java
  - Python
- Platform support:
  - Linux
  - MacOS
- Third party solver source archives:
  - [CBC](https://github.com/coin-or/cbc): COIN-OR branch and cut mixed integer program (MIP) solver
  - [SCIP](https://www.scipopt.org/doc/html/GETTINGSTARTED.php): Solving
    Constraint Integer Program, mixed integer linear and nonlinear program solver
- Build files:
  - [Bazel](https://bazel.build) build files
  - Make files
  - Binary tools for Windows
  - Docker files
  - F# project files

## Building the sources

Please download [CMake](https://cmake.org). CMake has excellent
[documentation](https://cmake.org/cmake/help/v3.22/). Try the
[tutorial](https://cmake.org/cmake/help/v3.22/guide/tutorial/index.html) to get
started with the tool. CMake is a two step build process. The first step
generates the build system, which is Visual Studio project files in our case.
The second step builds the targets using native build tools. You can also build
using Visual Studio or msbuild, or browse code using the generated .sln file.

The examples below assume that the current directory is the root of GLOP sources
(which would likely be the same directory as this Readme), and your desired
build directory is `./build`. 

Generate the build system, fetch and build dependent packages. No options are
required.

```
$ cmake -S . -B build
```

Build the release. The built packages will be in `build/dotnet/packages`.

```
$ cmake --build build --config Release
```

To directly import the package into your C# project, define a local source. This
step is not required if you are going to publish the package to a remote nuget
source.

`nuget.config`:
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="WANLP" value="path-to-tree/build/dotnet/packages" />
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    </packageSources>
</configuration>
```

Add the package to your C# project.

```
$ dotnet nuget update source "WANLP" --configfile nuget.config
$ dotnet nuget list source
$ dotnet add package Google.OrTools --version <4-number-version>
```

## Merging new code

Let's say you wish to merge new code from the public repository. The changes to
source files are maintained as patch files in `patches/wanlp_<n>_<text>.patch`
files. A helpful python script `patches/wanlp_rebase.py` automates the copy and
patch steps.

I would not try to merge any of the CMake code. The build system in this
repository has deviated significantly from the public repository. Also, our goal
of dotnet Windows build is narrower than the goal of public maintainers. Even if
source files are added or removed in listed ortools/ subdirectories, the CMake
files do not need any changes.

This is a snapshot of the execution of the rebase script. Let `private` be
the current private tree, `public` be the public repository with the new
code, and `new-private` be the new copy. This script will copy the relevant
files from the public and private trees, and apply the private patches.

```
$ python private\GLOP\patches\wanlp_rebase.py public private\GLOP new-private --apply_patches
Deleting existing new-private. Press [yY] to continue: y
Apply .\patches\wanlp-0-initial.patch, p to pick, c to check, any other letter to skip: p
```




