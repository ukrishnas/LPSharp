This project imports the following COIN-OR repositories as
[submodules](http://git-scm.com/book/en/v2/Git-Tools-Submodules):

- [BuildTools](https://github.com/coin-or-tools/BuildTools.git) has build
  headers.
- [CoinUtils](https://github.com/coin-or/CoinUtils.git) has general utilities
  used by the solver.
- [Clp](https://github.com/coin-or/Clp.git) has the solver.

The submodules have been modified in the following ways.

__What has been retained?__

- __BuildTools__ header files. This contains #defines for MSVC tool chain The
  library source directories have a copy and these are not strictly necessary.
- __CoinUtils__ source code and tests. CoinUtils library contains a number of
  common utility classes used by all COIN code including Clp.
- __LibClp__ source code and examples is the LP solver.
- __Clp__ standalone executable source code. This is an easy way to test and
  experiment with Clp solver, e.g. `clp.exe <mps file> -primal`. If invoked
  without arguments, it presents a simple but cryptic command line interface.
  Type `???` for full list of commands, and `command???` for help on command.


__What has been changed?__

- __Visual Studio__ files. The repositories had helpful Visual Studio project
  and solution files going back to version 9. We have just retained the latest
  version. Minor edits were made to the project files. Win32 build targets were
  removed from vcxproj files. We have now defined CMake files to match the
  vcxproj files and plan to remove these files.
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

__What has been added?__

- __ClpInterface__ is a more user friendly interface to Clp solver functions.
  The interface library is also linked as a standalone executable called `coinwrap.exe`.
- __CoinWrap__ is a SWIG and CMake based C# interface to Clp. It provides nuget
  packages that be imported into C# projects.
- __CMake__ build files for libClp and libCoinUtils. The solve times of `Clp.exe` are
  the same with Visual Studio and CMake.
  - The compiler directives and options were copied from the Visual Studio
    files.
  - C++ language standard is C++17.
  - BuildTools header files were excluded from the include path. They were not
    being used in the vcxproj builds.
  - Pre-compiled headers are not used, but the vcxproj files use them. It is meant
    for speeding up compilation, but compile times are pretty fast with multi-processor
    compilation, which is enabled for release builds. It cannot be enabled for debug
    builds due to error [C1090](https://docs.microsoft.com/en-us/cpp/error-messages/compiler-errors-1/fatal-error-c1090?view=msvc-170). 
  - ClCompile options ConformanceMode, FunctionLevelLinking,
    IntrinsicFunctions, SDLCheck are set to true in vcxproj but not defined in CMake.
  - Link options EnableCOMDATFolding, OptimizeReferences are set to true
    in vcxproj but not defined in CMake.
  - The size of libClp.lib and libCoinUtils.lib is 90MB with CMake, compared to
    70MB with vcxproj.
