# Networking-WANLP

WANLP is a repository of [linear
programming](https://en.wikipedia.org/wiki/Linear_programming) (LP) solvers and
LPSharp, a C# interactive test bench for these solvers. The supported solvers
are:

- CLP: Computation Infrastructure for Operations Research (COIN-OR) LP solver.
- GLOP: Google Operations Research Tools (OR-Tools) LP solver.
- MSF: Microsoft Solver Foundation LP solver.

The table below summarizes some information on the solvers.

||CLP|GLOP|MSF|
|--|--|--|--|
|Parent package|Computation Infrastructure for Operational Research (COIN)|Google Operational Research Tools (OR-Tools)|Microsoft Solver Foundation (MSF)|
|Creators|John Forrest (ex-IBM, still active)|Laurent Perron (Google, ex-IBM, still active)||
|First checkin|July 30 2002|September 15, 2010|Around 2006|
|Repositories with sizes|[Clp](https://github.com/coin-or/Clp) (35MB), [CoinUtils](https://github.com/coin-or/CoinUtils) (24MB), [BuildTools](https://github.com/coin-or-tools/BuildTools.git) (2MB)|[Or-tools](https://github.com/google/or-tools) (1.1GB)|[Private archive](https://microsoft.sharepoint.com/:u:/t/AzNet_WAN/EaP1nQ9PRwFOvMNDnozIAKsBsro8ubEwJFoW5SBWVK0R9Q?e=Eetqpg) (54MB)|
|Project status|Active|Active|Inactive|
|User guide|[User guide](https://coin-or.github.io/Clp/)|[User guide](https://developers.google.com/optimization/introduction/overview)|[Documents](https://microsoft.sharepoint.com/:f:/t/AzNet_WAN/EpklXccpFMhDvXQiSUuZkwsBtJbltaxKltKuO0MYzzZJqA?e=QKi3HS)|
|License|Eclipse Public License 2.0|Apache License 2.0|Microsoft proprietary|


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
repository and objects of our private changes. When a developer updates
submodules, the submodule git objects are cloned from the private remote. With
submodules, we can see the full history of the public repository maintainers,
and git tools assist in rebasing private changes on top of new updates.

__Copying__: OR-Tools is a monolithic, rather large repository of 1.1GB as of
October 2021. 95% of the size is consumed by examples. The maintainers of this
repository store binary files and tools in order to support multiple platforms.
There is a lot of excess fat in this repository. A large number of files had to
be stripped away from the public repository, and rebasing these changes can be
more work than applying a patch over the files we kept. Git operations, like
submodule update, clone, and  pull, become slow and require more network
bandwidth. In such cases, we have resorted to keeping a copy of the code and
patches of our changes. This approach can be reversed once the public repository
becomes modular.

__Platform support__: WANLP is targeted to the Windows operating system, x64
processor architecture, C#/.Net and C++/native platforms. Where solvers support
other operating systems like Linux and MacOS, or language wrappers like Python
and Java, the functionality for other platforms has been removed to make the
source code easier to understand. Where solvers do not natively support C# (e.g.
CLP), new SWIG wrappers have been developed and checked into this repository.

__Build system__: The build system of each project is different. CLP provides
GNU
[autotools/automake](https://www.rpi.edu/dept/cis/software/g77-mingw32/info-html/configure.html)
and Visual Studio builds, and the latter perfectly fits our needs. GLOP provides
[Bazel](https://bazel.build), [CMake](https://cmake.org), and make build
systems. CMake is best suited for our needs. It is easy to use, Bazel 4.2.1 does
not support C# builds and make is dated. MSF supports Visual Studio
builds. Build files for other build systems have been excluded to keep the code
base simple.

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

__LPSharp__ is a tool written in C# that provides a Powershell scripting
interface to the LP solvers. It uses a C# programming interface to the solvers
and hence is a proof-point that the LP solvers can be used by applications
written in C#.


## Submodule cheat sheet

This project uses
[submodules](http://git-scm.com/book/en/v2/Git-Tools-Submodules) for some code.
This means that some subdirectories in this project are separate git
repositories. These are tips for working with submodules.

__Clone superproject__. First, clone the project.
```
$ git clone https://github.com/ukrishnas/Networking-WANLP.git WANLP
Cloning into 'WANLP2'...
remote: Enumerating objects: 26912, done.
remote: Counting objects: 100% (26912/26912), done.
remote: Compressing objects: 100% (5851/5851), done.
```

__Update submodules__. Use `git submodule update` to update submodules. This
will populate the directories of the submodules. Notice that the URLs do not
point to the public repositories. We have changes in the submodules which cannot
be upstreamed to the public repository. Hence we have changed the URL in
`.gitmodules` to a private URL. In our example, it happens to be the same URL as
the parent project but the submodule repository is still separate.
```
$ cd WANLP
$ git submodule update --init --recursive

Submodule 'CLP/BuildTools' (https://github.com/ukrishnas/Networking-WANLP.git) registered for path 'BuildTools'
Submodule 'CLP/Clp' (https://github.com/ukrishnas/Networking-WANLP.git) registered for path 'Clp'
Submodule 'CLP/CoinUtils' (https://github.com/ukrishnas/Networking-WANLP.git) registered for path 'CoinUtils'
Cloning into 'WANLP/CLP/BuildTools'...
Cloning into 'WANLP/CLP/Clp'...
Cloning into 'WANLP/CLP/CoinUtils'...
Submodule path 'BuildTools': checked out 'a8720e29aa7db47f0127550a2644386d3ef95d5d'
Submodule path 'Clp': checked out '99781af262c5bad38c35fdd36d463c6337d69590'
Submodule path 'CoinUtils': checked out 'e368cc9dc5b95a1326f246c7d3f6b9dcc94eb66a'
```

 __Detached HEAD__. You will be in detached HEAD mode in each submodule. This is
normal. A detached HEAD means there is no local branch, and is fine if you are
going to simply build with the submodule.

```
$ cd CLP\clp
$ git status
HEAD detached at 99781af2
nothing to commit, working tree clean

$ git log -1
commit 99781af262c5bad38c35fdd36d463c6337d69590 (HEAD, origin/wanlp_clp_clp_master)
```

__Checkout__. If you wish to do development in the submodule, you need to
checkout a branch. The naming convention of the origin master branch for each
submodule is `wanlp_<folder>_<folder>_master`, all in lowercase. Checkout a
branch based off the submodule-specific master branch in the remote.

```
$ git checkout wanlp_clp_clp_master
Switched to a new branch 'wanlp_clp_clp_master'
Branch 'wanlp_clp_clp_master' set up to track remote branch 'wanlp_clp_clp_master' from 'origin'.
```

__Reset submodule__. If you need to restart your work on a submodule, you can
de-initialize and initialize the submodule. After de-initialize, the directory
will become empty. After update with the initialize option, the directory will
be reset to the tip in the remote.

```
$ git submodule deinit -f Clp
$ git submodule update --init Clp
```

__Update from public repository__: Let's say we want to pull changes made by the
maintainers of the public repository. We will fetch their changes, rebase our
private changes, and update our private remote for other users. Rebase is better
than merge in this scenario since it gives a linear history. The name `origin`
refers to the private remote, and `public_origin` refers to the public remote.

- Create a branch tracking the private tip of the submodule.
- Add the public repository as a remote and fetch new updates.
- Rebase private changes onto the new tip. Let `public_old_tip` be the old public
  tip on which we put our changes, and `public_new_tip` be the new public tip.
  Rebase will replay our private changes from the old tip onto the new tip.
- Commit and push changes.

```
$ cd Clp
$ git checkout -b clp_update -t origin/wanlp_clp_clp_master
$ git remote add public_origin https://github.com/coin-or/Clp.git
$ git fetch --all
$ git rebase --interactive --onto public_origin/<public_new_tip> public_origin/<public_old_tip> clp_update
$ git commit -a -m "Rebase private changes onto new public tip"
$ git push
```

