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


## Dotnet C# support

These are some choices for adding C# language support to Clp.

- __Sonnet__. Jan-Willem Goosens maintains
  [Sonnet](https://github.com/coin-or/Sonnet), that provides C# support to Clp.
  He started development in November 23, 2011. SonnetWrapper/ implements C++
  wrappers over Clp and Cbc classes to provide automatic garbage collection.
  Sonnet/ is C# code that calls the wrappers.

- __OR-Tools__. We could link libClp into Google.OrTools package and use this
  solver by invoking the MPInterface
  `Solver.CreateSolver("CLP_LINEAR_PROGRAMMING")`. This reproduces the approach
  envisaged by OR-Tools developers. Some Clp options are not exposed by
  clp_interface.cc. For example, we may want to use ClpSolve::automatic
  algorithm or call ClpSolve::setSpecialOption() to replicate the good benchmark
  performance of `Clp.exe -either`, but this would require enhancements to
  clp_interface.cc.

- __SWIG wrappers__. There are multiple projects that provide SWIG based
  wrappers to COIN solvers. The takeaway is that using SWIG for Clp is feasible.
  - [Cbcpy](https://gitlab.com/ikus-soft/cbcpy) is a SWIG based python wrapper
    to Cbc (COIN branch and cut mixed integer program solver).
  - [JniCbc](https://github.com/babakmoazzez/jCbc) is a Java native interface
    for Cbc and Clp.
  - [swIMP](http://swimp.sourceforge.net/) is a SWIG based Java wrapper to any
    open solver interface solver which includes Clp.

## CoinWrap

CoinWrap is our in-house development that uses ideas from Google OR-Tools, Clp
standalone and library code. Like OR-Tools, it implements a different
ClpInterface that encapsulates the solver functions and types that we need,
generates C# wrappers using SWIG, and packages them using the CMake build system
generator. The advantage of CoinWrap is it allows us to use the full feature set
of Clp but packaged in a more easy to use way that suits our needs. It will be
easier for us to maintain once developed. Building CoinWrap will generate dotnet
and native nuget packages that can be linked with C# projects.

A few miscellaneous points on CoinWrap:

  - This work is under development.
  - CoinWrap coding style follows the [Google C++ style
    guide](https://google.github.io/styleguide/cppguide.html).
  - Build using CMake. Do `cmake -S . -B build` to generate the build system,
    and `cmake --build build --config Release` to build the release target on a
    Windows platform. Build system expects libClp.lib and libCoinUtils.lib in
    their respective build folders.
  - The code borrows ideas from Google OrTools
    [clp_interface.cc](../GLOP/ortools/linear_solver/clp_interface.cc), its
    CMake build system, Clp standalone [ClpSolver.cpp](Clp/src/ClpSolver.cpp),
    and obviously the rest of Clp code base. As far as I could, I have not
    copied code, but  C++ code and build system may look similar. The CoinWrap
    code is licensed under Eclipse Public License to conform to the rest of COIN
    code base. Please check if this is correct since Google OR-Tools is licensed
    under the Apache License.