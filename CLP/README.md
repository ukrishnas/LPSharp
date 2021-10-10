# COIN-OR LP Solver

## Overview
This is the code of Computational Infrastructure for Operational Research
(COIN-OR) linear programming (CLP) solver. The code is pulled from the following repositories:

- [BuildTools](https://github.com/coin-or-tools/BuildTools.git) for build
  headers.
- [CoinUtils](https://github.com/coin-or/CoinUtils.git) for general utilities used
  the solver.
- [Clp](https://github.com/coin-or/Clp.git) for the solver.

The code from the public repositories have been simplified in the following
ways:

- Coinbrew is a helpful shell script and .coin-or folders that fetches dependent
  projects, runs configure and make. Since all files are already in a single
  respository, these files have been eliminated.
- The Open solver interface of CLP (libOsiClp) implements a proposed standard
  solver interface but it does not have much traction and we plan to directly
  call solver native APIs. Hence, libOsiClp from the Clp repository has been
  excluded.
- GNU automake and autoconf files (with .ac, .in, .m4 file endings) have been
  stripped from all repositories. This was done to make the source code easier
  to understand.
- The repositories had helpful Visual Studio project and solution files going
  back to version 9. We have just retained the latest version. Minor edits were
  made to the project files.

## SWIG

We need to add SWIG files to generate C# language bindings to the C++ solver
native APIs. This section needs to be filled in.
