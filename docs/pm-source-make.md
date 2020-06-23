


# The Source Directories Makefile.am Files

The source code directories are usually the `src` directory in a project's subdirectory, or subdirectories under `src`.

The Makefiles in these directories take care of the fun stuff.  They define what is to be built (libraries, executables), how it is built, and where (and if) the products are to be installed.

-----------------------------


## Beginning of the Makefile.am File

```
# Copyright (C) 2011 International Icecream Machines and others.
# All Rights Reserved.
# This file is distributed under the Eclipse Public License.

## $Id: Makefile.am 4242 2011-04-01 11:11:11Z johndoe $

# Author:  John Doe           IIM    2011-04-01

AUTOMAKE_OPTIONS = foreign

if COIN_HAS_OSI
  SUBDIRS = OsiClp
endif
```

 * As usual, we start with a *copyright note, author information*, and the *`svn:keywords` "`$Id$`"*.  I'm not sure if the *AUTOMAKE_OPTIONS* variable has to be set here...

   If their are further subdirectories with Makefiles that should be recursed into, then the *SUBDIRS* variable should be set.

------------------------------------


## Building a Library


### Name of the Library

```
# Name of the library compiled in this directory.
# We want it to be installed in the 'lib' directory.
lib_LTLIBRARIES = libClp.la
```

 * First we tell Automake the *name of the library* that we want to create.  The extension "`.la`" is used, since we are using the Libtool utility to handle libraries, which allows us to build both static and shared libraries on almost all platforms.  The final name of the library(ies) depends on the platform.

 * In the above example, we use the Automake prefix *lib_* to tell Automake that we want this library *to be installed in the lib installation directory*.  As the Automake primary we use *LTLIBRARIES* (as opposed to `LIBRARIES`) to tell Autoconf that we want to use Libtool.

 * If we want to build a library that is not going to be installed, we use the *noinst_* prefix.  For example, we might have the source code for the final library distributed into different directories, but in the end we only want to install one library that contains everything.  In this case, we need *to create auxiliary libraries* in each source code directory (except for one), that are later collected into a single library.  The auxiliary libraries should not be installed.

 The corresponding line in the `Makefile.am` file for an auxiliary library then looks like:
```
# Name of the library compiled in this directory.
# We don't want it to be installed since it will be collected into the libCgl library.
noinst_LTLIBRARIES = libCglTwomir.la
```


### Source Files for the Library

```
# List all source files for this library, including headers
libClp_la_SOURCES = \
        ClpConfig.h \
        ClpCholeskyBase.cpp ClpCholeskyBase.hpp \
        ClpCholeskyDense.cpp ClpCholeskyDense.hpp \
        ClpCholeskyUfl.cpp ClpCholeskyUfl.hpp \
        Clp_C_Interface.cpp Clp_C_Interface.h \
        ClpParameters.hpp \
.
.
.
```

 * To tell Automake which source files should be compiled in order to build the library, we use the *library name as prefix* (with "`.`" and "`-`" replaced by "`_`"), and the *SOURCES* primary.  In this list, one also includes all corresponding header files, so that they would be installed in a tarball generated by `make dist`.

 Based on the extension of the source files, `automake` will figure out how they are to be compiled.  In COIN-OR, we use *".c" for C code*, *".cpp" for C++ code*, and *".f" for Fortran code*.


### Additional Link Command for the Library

```
# This is for libtool
libClp_la_LDFLAGS = $(LT_LDFLAGS)
```

The `LDFLAGS` variable specifies *additional flags* that should be used when linking the library.
Here we set it to the `LT_LDFLAGS`, which is setup by configure.
If the project is a point release, then it contains a libtool option to *specify the version number in libtool* format, which is then used in the name of the generated library.


### Collecting Objects from Other Libraries

```
# We want to have also the objects from the DylpStdLib in this library
libDylp_la_LIBADD = ../DylpStdLib/libDylpStdLib.la

# Since automake is not doing this on its own, we need to declare the
# dependencies to the subdirectory libraries here
libDylp_la_DEPENDENCIES = $(libDylp_la_LIBADD)
```

 * If you distribute the source code for a library into several directories, you will have to tell Automake, which libraries should be included in the final library that is going to be installed.  In the above example, taken from the DyLP project, the final library to be installed is `libDylp` (with the appropriate extension, such as "`.a`").  In the `Makefile.am` for this library you would find these lines.  The *LIBADD* primary tells Autoconf, that the objects from the specified Libtool libraries should be included in the final library.  We also need to tell automake explicitly that the `libDyLP.la` library needs to be updated if there are changes to any of the component libraries included with `LIBADD`.

---------------------------------


## Building a Program


### Name of the Program

```
# Name of the executable compiled in this directory.
# We want it to be installed in the 'bin' directory.
bin_PROGRAMS = clp
```

 * First we tell Automake the *name of the executable* that we want to create (skip the "`.exe`" extension even if you are working under Windows).  Usually, we want programs to be installed in the `bin` installation directory, as indicated by the *bin_* prefix.  However, the unit test program is usually not installed, in which case one uses the *noinst_* prefix.


### Source Files for the Executable

```
# List all source files for this executable, including headers
clp_SOURCES = \
        ClpMain.cpp \
        CbcOrClpParam.cpp CbcOrClpParam.hpp \
        MyEventHandler.cpp MyEventHandler.hpp \
        MyMessageHandler.cpp MyMessageHandler.hpp \
        unitTest.cpp
```

 * Just as for libraries, one lists all source files, including headers, in a variable which has the *program name as prefix* and uses the *SOURCES* primary.


### Specifying Linking Flags

```
# List all additionally required libraries
clp_LDADD = libClp.la $(CLPLIB_LIBS)

# List all dependency libraries (similar to LDADD, but without -l, -L flags)
clp_DEPENDENCIES = libClp.la $(CLPLIB_DEPENDENCIES)

# Finally, the -rpath flag is used by libtool to make sure that the shared
# library is found (in the lib install directory) when we are using dynamic
# libraries.
clp_LDFLAGS = -rpath $(libdir)


# List all additionally required COIN-OR libraries
clp_LDADD = libClp.la \
        $(COINUTILSOBJDIR)/src/libCoinUtils.la

# Here we add additional libraries
LIBS += $(ADDLIBS) `cat $(COINUTILSOBJDIR)/coinutils_addlibs.txt`
```

 * The *_LDADD* variable is used to specify all library dependencies of a the binary (using the libtool extension "`.la`"). For the `clp` binary, we specify here the Clp library (of course), and all libraries that are required to link against the Clp library. The latter are [accumulated by configure](./pm-project-config) in the `CLPLIB_LIBS` variable.

 * The *_DEPENDENCIES* variable is used by make to generate dependency rules for the binary, so that it is recompiled if one of its dependencies is modified. The default mechanism of automake for setting up the `_DEPENDENCIES` variable does not work for us, since it sets up this variable based on the `_LDADD` variable at the time automake is executed. However, at this time, the values of the `CLPLIB_LIBS` variable is not known, so the dependencies would be incomplete. 

  However, the configure macros that also assemble the library flags in `CLPLIB_LIBS` also provide a variable `CLPLIB_DEPENDENCIES` which is the same as the `CLPLIB_LIBS` variable, but with everything that does not look like a the name of a library file removed, esp. flags starting with `-l` and `-L`.

----------------------------


## Additional Flags


### Include Directories

```
########################################################################
#                            Additional flags                          #
########################################################################

# Here list all include flags, relative to this "srcdir" directory.  This
# "cygpath" stuff is necessary to compile with native compilers on Windows.

AM_CPPFLAGS = $(COINDEPEND_CFLAGS) \
        -I`$(CYGPATH_W) $(srcdir)/..`
```

 * To specify the compiler flags for include directories for header files, one should use the *AM_CPPFLAGS* variable. The [macros that check for other projects in configure](./pm-project-config) already setup a variable that contain all compiler flags (esp. specifications of include directories) necessary to build against these projects.

 Additionally, we may have to specify other directories with header files in this project, which can be done as shown here. The usage of the *CYGPATH_W* variable might seem a bit cumbersome (and it is), but this is necessary to ensure that the code can also be compiled with native Windows compilers under Cygwin.  The `CYGPATH_W` variable is automatically set to "`cygpath -w`" on Cygwin, which translates the UNIX-style path to a proper Windows path.  On other platforms, it is simply set to "`echo`".


### Additional Preprocessor Definitions

```
# List additional defines
AM_CPPFLAGS += -DCOIN_NO_CLP_MESSAGE -DUSE_CBCCONFIG
```

 * Additional "`-D`" preprocessor flags should also be added to the *AM_CPPFLAGS* variable.


### Correction for Default Include Flags

```
# This line is necessary to allow VPATH compilation
DEFAULT_INCLUDES = -I. -I`$(CYGPATH_W) $(srcdir)` -I$(top_builddir)/src
```

 * You should have the lines above somewhere in your `Makefile.am` file, to make sure that users can compile your code in a VPATH configuration.  The default setting for *DEFAULT_INCLUDES* does not use the `CYGPATH_W` variable and does not add the include for the src subdirectory of the build directory.
 The latter is needed to find [automatically generated config header files](./pm-config-header) (provided they are put into this directory).

-------------------------


## Installation of Header Files

```
########################################################################
#                Headers that need to be installed                     #
########################################################################

# Here list all the header files that are required by a user of the library,
# and that therefore should be installed in 'include/coin'
includecoindir = $(includedir)/coin
includecoin_HEADERS = \
        CbcBranchActual.hpp \
        CbcBranchBase.hpp \
        CbcBranchLotsize.hpp \
        CbcBranchCut.hpp \
        CbcCompareActual.hpp \
        CbcCompareBase.hpp \
        CbcCutGenerator.hpp \
        CbcEventHandler.hpp \
        CbcHeuristic.hpp \
        CbcHeuristicFPump.hpp \
        CbcHeuristicGreedy.hpp \
.
.
.
```

 * In order to use a COIN-OR library (if it is written in C or C++), a user will need some of the header files in the source directories to compile her/his own code.  For this reason, we specify the required header files (which might only be a subset of all header files in the source directory) in the *_HEADERS* Automake variable.  The specification of *includecoindir* and the prefix *includecoin_* tells `automake` that these files should be copied into the coin subdirectory of the `include` installation directory.

 The configuration header file is installed separately (next point).

```
nobase_includecoin_HEADERS = foo.h bar/bar.h
```

 * If you need header files to be installed into subdirectories of the include directory instead of the default _flat_ directory structure, you can use the prefix *nobase_includecoin_* instead of *includecoin_*.
   In the example, the file `foo.h` is copied to `include/coin/foo.h`, while the file `bar/bar.h` is copied to `include/coin/bar/bar.h`.

```
install-exec-local:
        $(install_sh_DATA) config_clp.h $(DESTDIR)$(includecoindir)/ClpConfig.h

uninstall-local:
        rm -f $(DESTDIR)$(includedir)/ClpConfig.h
```

 * As discussed in the [Configuration Header files page](./pm-config-header), in COIN-OR we don't include the configuration header files `config*.h` directly.  Instead, this is done via the *_Pkg_Config.h* file, to make sure that the compilation can also be done smoothly in a non-autotools setup. However, when building against an installed version of a project, only the _public_ configuration header file is required. Thus, the above lines ensure that the public header is installed as *_Pkg_Config.h* file. *Do not install the _private_ configuration header file `config.h`*, recall the information [here](./pm-config-header).
 The `install-exec-local` is run by the generated Makefile for a `make install`, and the commands for `uninstall-local` are executed for the `make uninstall`.