# COIN-OR LP Solver

This folder contains the code of Computational Infrastructure for Operational
Research (COIN-OR) linear programming (CLP) solver. 

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

- BuildTools header files
- CoinUtils source code
- LibClp source code
- Clp executable source code
- LibClp examples
- Visual Studio build files

__What has been changed?__

- The repositories had helpful Visual Studio project and solution files going
  back to version 9. We have just retained the latest version. Minor edits were
  made to the project files.

__What has been removed?__

- Coinbrew is a helpful shell script and .coin-or folders that fetches dependent
  projects, runs configure and make. Since all files are already in a single
  repository, these files have been eliminated.
- The Open solver interface of CLP (libOsiClp) implements a proposed standard
  solver interface but it does not have much traction and we plan to directly
  call solver native APIs. Hence, libOsiClp from the Clp repository has been
  excluded.
- GNU automake and autoconf files (with .ac, .in, .m4 file endings) have been
  stripped from all repositories. This was done to make the source code easier
  to understand.


## Dotnet C# support

Jan-Willem Goosens maintains [Sonnet](https://github.com/coin-or/Sonnet), which
is a project that provides C# support to Clp. He started development in November
23, 2011. This is our first candidate for importing C# support for CLP.

A second option could be to using SWIG to generate C# wrappers and implement an
interface.

With either approach, the output of this work will be dotnet and native nuget packages.

