


# The Test Directory Makefile.am File

In the `test` subdirectory, which should provide a unit test, the Makefile has a *target test* which compiles and runs the unit test.


## Beginning of the File

```
# Copyright (C) 2011 International Icecream Machines and others.
# All Rights Reserved.
# This file is distributed under the Eclipse Public License.

## $Id: Makefile.am 788 2011-04-01 11:11:11Z johndoe $

# Author:  John Doe              IIM    2011-04-01

AUTOMAKE_OPTIONS = foreign
```

 * As usual, we should have some introduction with *copyright note, author information, etc.*


## Compilation of the Unit Test Program

```
########################################################################
#                      unitTest for CoinUtils                          #
########################################################################

noinst_PROGRAMS = unitTest

unitTest_SOURCES = \
        CoinDenseVectorTest.cpp \
        CoinErrorTest.cpp \
        CoinIndexedVectorTest.cpp \
        CoinMessageHandlerTest.cpp \
        CoinModelTest.cpp \
        CoinMpsIOTest.cpp \
        CoinPackedMatrixTest.cpp \
        CoinPackedVectorTest.cpp \
        CoinShallowPackedVectorTest.cpp \
        unitTest.cpp

# List libraries to link into binary
unitTest_LDADD = ../src/libCoinUtils.la $(COINUTILSLIB_LIBS)

# Dependencies of binaries are mostly the same as given in LDADD, but with -l and -L removed
unitTest_DEPENDENCIES = ../src/libCoinUtils.la $(COINUTILSLIB_DEPENDENCIES)

# Here list all include flags, relative to this "srcdir" directory.
# This "cygpath" stuff is necessary to compile with native compilers on Cygwin.
AM_CPPFLAGS = -I`$(CYGPATH_W) $(srcdir)/../src`

# This line is necessary to allow VPATH compilation with MS compilers on Cygwin.
DEFAULT_INCLUDES = -I. -I`$(CYGPATH_W) $(srcdir)` -I$(top_builddir)/inc
```

 * The above example is taken from the CoinUtils project.  It follows the same scheme as describe for the [source directory Makefile.am file](./pm-source-make) for defining the compilation of a program.  Since we don't want to install the `unitTest` program, we use the `noinst_` prefix for the `PROGRAMS` primary.


## The Test Target

```
unittestflags =
if COIN_HAS_SAMPLE
  unittestflags += -mpsDir=`$(CYGPATH_W) $(SAMPLE_DATA)`
endif
if COIN_HAS_NETLIB
  unittestflags += -netlibDir=`$(CYGPATH_W) $(NETLIB_DATA)` -testModel=adlittle.mps
endif

test: unitTest$(EXEEXT)
        ./unitTest$(EXEEXT) $(unittestflags)

.PHONY: test
```

 * The *test target* above depends on the `unitTest` executable.  Note the addition of the *EXEEXT* variable to the executable name; this variable is set to the extension of executables on the specific platform.  For example, on UNIX systems it is an empty string, and on Windows it is automatically set to "`.exe`".

 For this particular test, the executable will use files of the `Data/Sample` and `Data/Netlib` projects, if available. The paths under which the data of these projects can be found has been setup by the [configure script in the project directory](./pm-project-config) and stored in the `SAMPLE_DATA` and `NETLIB_DATA` variables.


## House Cleaning

```
########################################################################
#                          Cleaning stuff                              #
########################################################################

# Here we list everything that is not generated by the compiler, e.g.,
# output files of a program

DISTCLEANFILES = yy.mps xx.mps
```

 * The unit test program might generate output files.  It is a nice gesture to the user to make sure that everything is cleaned up, when (s)he does a `make distclean`.  For this reason, you should list all possibly generated output files in the *DISTCLEANFILES* Automake variable.