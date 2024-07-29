The capabilities of Clp are quite extensive, but arcane. Starting basis methods,
pivot algorithms, enabling plus-minus one matrix, using automatic algorithm,
etc. require setting special options and more special options with integers
representing bit fields and extra information. CoinWrap consists of a Clp
interface which is a more user friendly interface to a subset of Clp
functionality from ClpSolver (the source code for Clp standalone executable),
ClpSimplex (the solver driver), ClpSolve (the solver options), CoinBuild (to
build models), and related classes.

It uses SWIG to generate C# language wrappers. This can be extended to other
languages in the future. CMake is used to compile and build dotnet nuget
packages for Windows x64 platforms only.

CoinWrap allows us to use the full feature set of Clp but packaged in a more
easy to use way that suits our needs. The implementation uses Clp natively and
not the open solver interface, in order to get full access to solver functions.

A few miscellaneous points on CoinWrap:

  - CoinWrap was inspired by Google OrTools approach to generating
    MPSolverInterface wrappers using SWIG and dotnet nuget packages using CMake.
  - CoinWrap coding style follows the [Google C++ style
    guide](https://google.github.io/styleguide/cppguide.html).


## Related work

- [__Sonnet__](https://github.com/coin-or/Sonnet), maintained by Jan-Willem Goosens,
  provides C# support to Clp. SonnetWrapper/ implements C++ wrappers over Cbc,
  Clp, and OsiClp classes, and Sonnet/ is C# code that calls the wrappers.

- [__Google OrTools__](../GLOP/ortools) provides multi-language (including C#)
  support to its linear solver interface, called MPSolverInterface, and an
  [implementation](../GLOP/ortools/linear_solver/clp_interface.cc) of the
  interface using ClpSimplex, ClpSolve, and CoinBuild classes. They use SWIG to
  generate language wrappers.

- Both [Sonnet](https://github.com/coin-or/Sonnet) and [Google
  OrTools](../GLOP/ortools) lack fine control for modifying Clp solver
  parameters. For example, we may want to use ClpSolve::automatic algorithm,
  call ClpSolve::setSpecialOption() to replicate the good benchmark performance,
  but these would require enhancements to the interfaces.

- __SWIG wrappers__. Multiple projects provide SWIG based wrappers to COIN
  solvers, but do not have exactly what we need.
  - [Cbcpy](https://gitlab.com/ikus-soft/cbcpy) is a SWIG based python wrapper
    to Cbc (COIN branch and cut mixed integer program solver).
  - [JniCbc](https://github.com/babakmoazzez/jCbc) is a Java native interface
    for Cbc and Clp.
  - [swIMP](http://swimp.sourceforge.net/) is a SWIG based Java wrapper to any
    open solver interface solver which includes Clp.

## CoinWrap cheatsheet

The first argument is the model file. The second argument is the recipe and
choices are: barrier, dualcrash, duals, either, primals, primalidiot (all in
lower case).

```
$ coinwrap.exe netlib\80bau38.mps either
$ coinwrap.exe netlib\80bau38.mps duals
$ coinwrap.exe netlib\80bau38.mps primals
```
