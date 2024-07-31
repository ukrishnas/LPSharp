Let's say you wish to merge new code from the public repository. The changes to
source files are maintained as patch files in `patches/glop_<n>_<text>.patch`
files, `n` is the patch order, and `text` is a short description. A helpful
python script `patches/glop_rebase.py` automates the copy and patch steps.

I would not try to merge any of the CMake code. The build system in this
repository has deviated significantly from the public repository. Also, our goal
of dotnet Windows build is narrower than the goal of public maintainers. Even if
source files are added or removed in listed ortools/ subdirectories, the CMake
files do not need any changes.

This is a snapshot of the execution of the rebase script. Let `private` be
the current private tree, `public` be the public repository with the new
code, and `new-private` be the new copy. This script will copy the relevant
files from the public and private trees, and apply the private patches.

```
python private\GLOP\patches\glop_rebase.py public private\GLOP new-private --apply_patches
```

Output.

```
Deleting existing new-private. Press [yY] to continue: y
Apply .\patches\glop-0-initial.patch, p to pick, c to check, any other letter to skip: p
```