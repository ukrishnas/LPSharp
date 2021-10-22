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

The submodules have been simplified in the following ways.

__What has been retained?__

- __BuildTools__ header files. This contains #defines for MSVC tool chain.
- __CoinUtils__ source code and tests. CoinUtils library contains a number of
  common utility classes used by all COIN code including Clp.
- __LibClp__ source code and examples.
- __Clp__ standalone executable source code. This is an easy way to test Clp
  solver, e.g. `clp.exe <mps file> --either`. If invoked without arguments, it
  presents a simple but cryptic command line interface.
- __Visual Studio__ 2019 build files.

__What has been changed?__

- The repositories had helpful Visual Studio project and solution files going
  back to version 9. We have just retained the latest version. Minor edits were
  made to the project files.
- Win32 build targets were removed.
- Missing #ifdef CLP_HAS_ABC was added in a few places.

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
- __Clp Aboca__ solver. These are Abc* and CoinAbc* files in the Clp repository
  src/ folder. References to this code is under #ifdef CLP_HAS_ABC. I do not
  know what it is, but these files were first added in 2012, and was not part of
  the Clp Visual Studio project file to begin with.  We removed these files to
  make the source tree easier to understand. If you wish to build Clp with
  `--enable-aboca`, then you will need to reinstate these files.


## Dotnet C# support

This work is under development. The result of this work will be dotnet and
native nuget packages that can be linked with C# projects.


- Jan-Willem Goosens maintains [Sonnet](https://github.com/coin-or/Sonnet), that
  provides C# support to Clp. He started development in November 23, 2011.
  SonnetWrapper/ implements C++ wrappers over Clp and Cbc classes to provide
  automatic garbage collection. Sonnet/ is C# code that calls the wrappers.

- Google OR-Tools
  [clp_interface.cc](https://github.com/google/or-tools/blob/stable/ortools/linear_solver/clp_interface.cc)
  implements MPInterface (a common solver interface) using ClpSimplex (the
  solver class) and ClpSolve (class for options). It instantiates these classes
  using std::unique_ptr so that C++ standard library would provide automatic
  garbage collection. It uses the following Clp native methods, which gives us
  an idea of the methods that need to be wrapped.

  - ClpSolve
    - setPresolveType
    - setSolveType
  - ClpSimplex
    - addColumn
    - getColumnStatus
    - getIterationCount
    - getRowPrice
    - getRowStatus
    - modifyCoefficient
    - passInMessageHandler (passes CoinMessageHandler)
    - resize
    - setColumnBounds
    - setColumnName
    - setDualTolerance
    - setLogLevel
    - setObjectiveCoefficient
    - setObjectiveOffset
    - setOptimizationDirection
    - setPrimalTolerance
    - setRowBounds
    - setRowName
    - setStrParam

- We could use SWIG to define C# (or for that matter any language) wrappers for
  the above methods and referenced types. This is approach taken by Google
  OR-Tools for its MPInterface. For example, we could modify clp_interface.cc to
  remove the MPInterface bits, write SWIG code for this modified interface, and
  leverage the CMake and dotnet bits to build the nuget packages.

- We could link libClp into Google.OrTools package and use this solver by
  invoking the MPInterface `Solver.CreateSolver("CLP_LINEAR_PROGRAMMING")`. This
  reproduces the approach envisaged by OR-Tools developers. Some Clp options are
  not exposed by clp_interface.cc. For example, we may want to use
  ClpSolve::automatic algorithm or call ClpSolve::setSpecialOption() to
  replicate the good benchmark performance of `Clp.exe -either`, but this would
  require enhancements to clp_interface.cc.