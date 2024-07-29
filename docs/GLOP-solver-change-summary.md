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
    Version.txt, patch is the count of commits in the public repository at the
    time of the merge, and tweak is the count of commits in the private
    repository.
  - Cpp.cmake builds and installs the C++ sources. This in turn uses
    CMakeLists.txt in each ortools/ subdirectory: base, glop, linear_solver,
    lp_data, port, and util.
  - Dotnet.cmake generates C# sources using Swig code in csharp/ folders in
    linear_sharp and util subdirectories. It generates dotnet build files using
    input files in ortools/dotnet. In the build step, it produces dotnet and
    native nuget packages. The dotnet package contains SWIG generated C# code.
    The native package contains the SWIG generated C++ code and previously
    compiled GLOP objects.
  - The names of the dotnet namespace, native runtime library, and both nuget
    packages been suffixed with 'glop', for example, Google.OrTools.Glop.LinearSolver,
    google-ortools-glop-native.dll.
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
- The examples folder has a standalone GLOP executable, that can be used to
  exercise the solver without SWIG wrappers. It also has better logging support.

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