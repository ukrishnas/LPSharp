# OR-Tools LP Solver

This folder contains the code for Google Operations Research Tools (OR-Tools)
linear programming (GLOP) solver.

## Change summary

Owing to the large and monolithic nature of the
[OR-Tools](https://github.com/google/ortools) public repository, the files in
this folder are a copy and not a git submodule. Many code modules have been
removed, and some have been changed. GLOP solver code depends on
[Abseil](https://github.com/abseil/abseil-cpp.git), [Protocol
Buffers](https://github.com/protocolbuffers/protobuf.git), and
[ZLib](https://github.com/madler/ZLIB.git). These are imported from their public
repositories during build.

__What has been retained?__

- GLOP solver code
- CMake build system
- SWIG support for C# language
- Platform support for dotnet on Windows only

__What has been changed?__

- CMake top-level file to alter defaults and remove unneeded functionality

__What has been removed?__

- Documentation and examples:
  - All documentation (doc[s]/ folders)
  - All examples (examples/ folder)
  - All sample code (samples/ folders)
- Functionality:
  - Interfaces for third-party solvers (COIN, GLPK, Gurobi, SCIP, Xpress)
  - Code for satisfiability and constrained programming solvers
  - Code for combinatorial optimization and graph algorithms
  - Code for vehicle routing
- Language support:
  - [Flat Zinc](https://www.minizinc.org/) modeling language
  - Java
  - Python
- Platform support:
  - Linux
  - MacOS
- Third party solver code:
  - [CBC](https://github.com/coin-or/cbc): COIN-OR branch and cut mixed integer program (MIP) solver
  - [SCIP](https://www.scipopt.org/doc/html/GETTINGSTARTED.php): Solving Constraint Integer Program MIP/MINP/solver
- Build files:
  - Make files
  - [Bazel](https://bazel.build) build files
  - Binary tools for Windows
  - F# project files
  - Docker files
