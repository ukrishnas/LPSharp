# LPSharp

LPSharp is a C# test bench and interface to [linear
programming](https://en.wikipedia.org/wiki/Linear_programming) (LP) solvers. The supported solvers
are:

- CLP: Computation Infrastructure for Operations Research (COIN-OR) LP solver.
- GLOP: Google Operations Research Tools (OR-Tools) LP solver.

Please see [documentation](docs/) for detailed information and guides. We have kept a short version
below to get you started. 

## Cloning the repository

This project uses [submodules](docs/Git-Submodule-cheatsheet.md) for some code.
This means that some subdirectories in this project are separate git
repositories. First, clone the project. 

```
git clone https://github.com/ukrishnas/LPSharp.git LPSharp
```

Next, update submodules.
```
cd LPSharp
git submodule update --init --recursive
```

## What is where

The following is an explanation of the contents of the top-level folders in the
source tree.

- CLP has the source code of COIN-OR LP solver. It contains BuildTools, Clp,
  CoinUtils from public repositories. In addition, it has CoinWrap, which we
  wrote as a more easy to use interface to Clp solver, and a SWIG-based C#
  wrapper to this interface.
- GLOP has the source code for Google OR-Tools LP solver. It contains a subset
  of the OR-Tools code base as a copy (until the public repository decreases in
  size). Only the LP solver related code has been retained.
- LPBench has benchmark results and logs of the solvers built from this
  repository using Netlib and the collection maintained by Hans D. Mittelmann.
- LPSharp is the C# based Powershell console that provides script-able cmdlets
  to read MPS files, instantiate multiple solvers, read solver-specific
  parameters, load models into the solver using solver APIs to create variables,
  constraints, and objective, solve the models, and enable logging of the
  optimization activity.

