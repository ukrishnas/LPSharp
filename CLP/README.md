# COIN-OR LP Solver

This folder contains the code of Computational Infrastructure for Operational
Research (COIN-OR) linear programming (Clp) solver. 

## Submodules

This superproject imports the following COIN-OR repositories as
[submodules](http://git-scm.com/book/en/v2/Git-Tools-Submodules):

- [BuildTools](https://github.com/coin-or-tools/BuildTools.git) for build
  headers.
- [CoinUtils](https://github.com/coin-or/CoinUtils.git) for general utilities used
  the solver.
- [Clp](https://github.com/coin-or/Clp.git) for the solver.

The submodules have been modified in the following ways.

__What has been retained?__

- __BuildTools__ header files. This contains #defines for MSVC tool chain.
- __CoinUtils__ source code and tests. CoinUtils library contains a number of
  common utility classes used by all COIN code including Clp.
- __LibClp__ source code and examples is the LP solver.
- __Clp__ standalone executable source code. This is an easy way to test and
  experiment with Clp solver, e.g. `clp.exe <mps file> -primal`. If invoked
  without arguments, it presents a simple but cryptic command line interface.
  Type `???` for full list of commands, and `command???` for help on command.
- __Visual Studio__ 2019 build files.

__What has been changed?__

- __Visual Studio__ files. The repositories had helpful Visual Studio project
  and solution files going back to version 9. We have just retained the latest
  version. Minor edits were made to the project files. Win32 build targets were
  removed from vcxproj files.
- __Clp Aboca__ solver. These are Abc* and CoinAbc* files in the Clp repository.
  References to this code is under #ifdef CLP_HAS_ABC. Missing #ifdef
  CLP_HAS_ABC was added in a few places. I do not know what it is, but these
  files were first added in 2012, and was not part of the Clp Visual Studio
  project file to begin with. We removed these files to make the source tree
  easier to understand. If you wish to build Clp with `--enable-aboca`, then you
  will need to reinstate these files.

__What has been removed?__

- __Coinbrew__ is a helpful shell script and .coin-or folders that fetches
  dependent projects, runs configure and make. Since all files are already in a
  single repository, these files have been eliminated.
- __LibOsiClp__ in the Clp repository is the open solver interface for Clp. This
  is another standard solver interface but we plan to directly call Clp native
  APIs.
- __GNU automake and autoconf__ files (with .ac, .in, .m4 file endings) have
  been stripped from all repositories. This was done to make the source code
  easier to understand.


## CoinWrap - C# interface to Clp

The capabilities of Clp are quite extensive, but arcane. Starting basis methods,
pivot algorithms, enabling plus-minus one matrix, using automatic algorithm,
etc. require setting special options and more special options with seeming
arbitrary integers representing bit fields and extra information. CoinWrap
consists of a Clp interface which is a more user friendly interface to a subset
of Clp functionality from ClpSolver (the source code for Clp standalone
executable), ClpSimplex (the solver driver), ClpSolve (the solver options),
CoinModel (to build models), and related classes.

It uses SWIG to generate C# language wrappers. This can be extended to other
languages in the future. CMake is used to compile and build dotnet nuget
packages for Windows x64 platforms only.

CoinWrap allows us to use the full feature set of Clp but packaged in a more
easy to use way that suits our needs. The implementation uses Clp natively and
not the open solver interface, like some other implementations, in order to get
full access to solver functions.

A few miscellaneous points on CoinWrap:

  - CoinWrap was inspired by Google OrTools approach to generating
    MPSolverInterface wrappers using SWIG. We used their CMake code to generate
    nuget packages.
  - CoinWrap coding style follows the [Google C++ style
    guide](https://google.github.io/styleguide/cppguide.html).
  - Build using CMake. Do `cmake -S . -B build` to generate the build system,
    and `cmake --build build --config Release` to build the release target on a
    Windows platform. Build system expects libClp.lib and libCoinUtils.lib in
    their respective build folders.
  - CoinWrap generates two nuget packages in the `build/dotnet/packages` folder.
    The native library has Clp interface, libClp, and libCoinUtils. The .Net
    library has the C# code. The version number is based on the public release
    number and the commit counts in the Clp and CoinWrap code bases.
  - Import the packages to your C# project using:
```
$ dotnet add package CoinOr.Clp --version <version number>
```

### Related work

- [__Sonnet__](https://github.com/coin-or/Sonnet), maintained by Jan-Willem Goosens,
  provides C# support to Clp. SonnetWrapper/ implements C++ wrappers over Cbc,
  Clp, and OsiClp classes, and Sonnet/ is C# code that calls the wrappers.

- [__Google OrTools__](../GLOP/ortools) provides multi-language (including C#)
  support to its linear solver interface, called MPSolverInterface, and an
  [implementation](../GLOP/ortools/linear_solver/clp_interface.cc) of the
  interface using Clp solver class. They use SWIG to generate language wrappers.

- Both [Sonnet](https://github.com/coin-or/Sonnet) and [Google
  OrTools](../GLOP/ortools) lack fine control for modifying Clp solver
  parameters. For example, we may want to use ClpSolve::automatic algorithm,
  call ClpSolve::setSpecialOption() to replicate the good benchmark performance,
  but these would require enhancements to the interfaces.

- __SWIG wrappers__. There are multiple projects that provide SWIG based
  wrappers to COIN solvers. The takeaway is that using SWIG for Clp is feasible.
  - [Cbcpy](https://gitlab.com/ikus-soft/cbcpy) is a SWIG based python wrapper
    to Cbc (COIN branch and cut mixed integer program solver).
  - [JniCbc](https://github.com/babakmoazzez/jCbc) is a Java native interface
    for Cbc and Clp.
  - [swIMP](http://swimp.sourceforge.net/) is a SWIG based Java wrapper to any
    open solver interface solver which includes Clp.

