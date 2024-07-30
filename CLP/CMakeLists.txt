cmake_minimum_required(VERSION 3.15)

# Include folder for additional cmake functions. 
list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/cmake")

# Come up with a project version. The version is used only in CoinWrap project.
include(version)
set_version(VERSION)
project(coinor VERSION ${VERSION})

# Set some global defaults that are used to initialize target properties when
# the targets are created.
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)
set(CMAKE_CXX_EXTENSIONS False)

# Build CoinUtils and Clp libraries, and Clp standalone executable.
include(cpp)
add_subdirectory(coinutils)
add_subdirectory(clp/src)
add_subdirectory(clp/examples)

# Build the Clp interface and C# nuget packages.
add_subdirectory(coinwrap)