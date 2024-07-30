LPSharp is a C# test bench and interface to [linear
programming](https://en.wikipedia.org/wiki/Linear_programming) (LP) solvers. The
supported solvers are:

- CLP: Computation Infrastructure for Operations Research (COIN-OR) LP solver.
- GLOP: Google Operations Research Tools (OR-Tools) LP solver.

The table below summarizes some information on the solvers.

||CLP|GLOP|
| -- | -- | -- |
| Parent package | Computation Infrastructure for Operational Research (COIN) | Google Operational Research Tools (OR-Tools) |
| Creators | John Forrest (ex-IBM, still active) | Laurent Perron (Google, ex-IBM, still active) |
| First checkin | July 30 2002 | September 15, 2010 |
| Repositories with sizes | [Clp](https://github.com/coin-or/Clp) ([35MB](https://api.github.com/repos/coin-or/clp)), [CoinUtils](https://github.com/coin-or/CoinUtils) ([24MB](https://api.github.com/repos/coin-or/coinutils)), [BuildTools](https://github.com/coin-or-tools/BuildTools.git) ([2MB](https://api.github.com/repos/coin-or-tools/buildtools)) | [Or-tools](https://github.com/google/or-tools) ([1.1GB](https://api.github.com/repos/google/or-tools)) |
| Project status | Active | Active |
| User guide | [User guide](https://coin-or.github.io/Clp/) | [User guide](https://developers.google.com/optimization/introduction/overview) |
| License | Eclipse Public License 2.0 | Apache License 2.0 |

__Motivation__: We started this repository to make use of open source LP solvers
in our wide area network traffic engineering solver. From a development
standpoint, we need to make solvers written in C++ work with C# code since most
of our code is written in C#. LP relaxation is often a means to mixed integer
programming or other optimizations, but traffic engineering optimization
problems, like approximate max-min fairness, diverse path allocation, and
minimizing maximum utilization are pure LP problems. We wanted to de-clutter the
code bases to make them easy to understand and maintain. There is a lot of
benefit if we can extract the full potential of solver for our models. Simply
running the solvers with default values is not enough. This requires us to gain
a deeper understanding of the LP code. These were the reasons, we started this
repository.

__LP solvers__: LP solvers are often part of larger optimization suites that
consist of [boolean
satisfiability](https://en.wikipedia.org/wiki/Boolean_satisfiability_problem)
(SAT), [constraint
programming](https://en.wikipedia.org/wiki/Constraint_programming) (CP), [mixed
integer
programming](https://en.wikipedia.org/wiki/Linear_programming#Integer_unknowns)
(MIP), [nonlinear
programming](https://en.wikipedia.org/wiki/Nonlinear_programming) and
[mathematical
optimization](https://en.wikipedia.org/wiki/Mathematical_optimization#Computational_optimization_techniques)
solvers, and combinatorial optimization algorithms. LP solvers are a means to
solve LP relaxations in these more interesting problems. We have retained just
the LP solver code to make this project easy to understand, maintain, and clone
for its developers. A small subset of examples and samples have been retained
for unit tests.

__Submodules__: [Git
submodules](http://git-scm.com/book/en/v2/Git-Tools-Submodules) are the
preferred method to import solver code in public repositories.  Some open-source
solvers, e.g. CLP, are nicely packaged into separate repositories. In such
cases, the public repositories are included as git submodules. Since we make
private changes to the solver code that cannot be upstreamed, like removing
unneeded code, we push the changes to a private remote that happens to be this
very repository. The submodule repository contains git objects of the public
repository and objects of our private changes. When you clone this repository
and update submodules, the submodule git objects are cloned from the private
remote. With submodules, we can see the full history of the public repository
maintainers, and git tools assist in rebasing private changes on top of new
updates.

__Copying__: OR-Tools is a monolithic, rather large repository of 1.1GB as of
October 2021. 95% of the size is consumed by examples. The maintainers of this
repository store binary files and tools in order to support multiple platforms.
There is a lot of excess fat in this repository. A large number of files had to
be stripped away from the public repository, and rebasing these changes can be
more work than applying a patch over the files we kept. Git operations, like
submodule update, clone, and pull, become slow and require more network
bandwidth. In such cases, we have resorted to keeping a copy of the code,
patches of our changes, and an interactive python copy and rebase script. This
approach can be reversed once the public repository becomes modular.

__Platform support__: LPSharp is targeted to the Windows operating system, x64
processor architecture, C#/.Net and C++/native platforms. Where solvers support
other operating systems like Linux and MacOS, or language wrappers like Python
and Java, the functionality for other platforms has been removed to make the
source code easier to understand. Where solvers do not natively support C# (e.g.
CLP), new support has been developed or imported into this repository.

__Build system__: We have standardized on [CMake](https://cmake.org) as the
build system for all solvers. It is easy to use and multi-platform. CLP provides
GNU
[autotools/automake](https://www.rpi.edu/dept/cis/software/g77-mingw32/info-html/configure.html)
and Visual Studio builds, and we have replaced these build systems with CMake.
GLOP provides [Bazel](https://bazel.build), CMake, and make build systems. Bazel
4.2.1 does not support C# builds and make is dated. Build files for other build
systems have been excluded to keep the code base simple.

__Dependencies__: Dependent libraries are downloaded and built as part of the
build process. They are not imported as submodules. CLP has no external
dependencies. GLOP solver code depends on
[Abseil](https://github.com/abseil/abseil-cpp.git), [Protocol
Buffers](https://github.com/protocolbuffers/protobuf.git), and
[ZLib](https://github.com/madler/ZLIB.git).

__Upstreaming changes__: Bug fixes and functional changes that will be useful to
the broader community should be upstreamed to the public repositories. C#
language support for CLP could be a candidate if the public maintainers are
amenable to it. Such changes should not be attempted in this repository. They
should be applied to a separately cloned public repository and submitted
according to the processes of the open source project.

__LPSharp Powershell console__ is a program written in C# that provides a
Powershell scripting interface to the LP solvers. It uses a C# programming
interface to the solvers and hence is a proof-point that the LP solvers can be
used by applications written in C#. The Powershell console has cmdlets to read
MPS files, instantiate multiple solvers, read solver-specific parameters, load
models into the solver using solver APIs to create variables, constraints, and
objective, solve the models, and enable logging of the optimization activity.
