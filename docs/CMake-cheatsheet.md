Please download [CMake](https://cmake.org).

CMake has excellent [documentation](https://cmake.org/cmake/help/v3.22/). The
language is precise, but may need multiple reads. The answer to my questions are
almost always in the documentation.

CMake is a two step build process. The first step generates the build system,
which is Visual Studio project files in our case. The second step builds the
targets using native build tools. You can also build using Visual Studio or
msbuild, or browse code using the generated .sln file.

Try the [tutorial](https://cmake.org/cmake/help/v3.22/guide/tutorial/index.html)
if you are going to edit CMakeLists files in the source tree.

## Miscellaneous points.

For help and version.

```
$ cmake -h
$ cmake --version
```

Visual Studio is a multi-configuration generator. You must specify the build
configuration in the command line as shown below. You cannot specify it using
CMAKE_BUILD_TYPE. 

``` 
$ cmake --build build --config release
$ cmake --build build --config debug
$ ctest -C release
```

When generating some csproj files in the build process, we use two steps.
Configure file handles variable names and runs in the configuration phase of
CMake (when you execute `cmake -S -B`). File generate handles generator
expressions and runs in the generate phase (when you execute `cmake --build`).

```
configure_file(... @ONLY)
file(GENERATE ...)
```

Variable defined with the `set()` command may appear empty when referenced in a
different file. This is due to the scope of the variable. A variable has the
scope in the function or directory. That is why we sometimes define variables
in parent file. For example, Clp and CoinUtils are different directory scopes,
and so we define the target names for the libraries in a common parent scope. In
other places, `PARENT_SCOPE` is used to achieve the same result.
