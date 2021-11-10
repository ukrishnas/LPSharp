cmake_minimum_required(VERSION 3.15)
project(coinwrap)

# Enable fetching SWIG.
set(BUILD_SWIG ON)
set(BUILD_TESTS OFF)

# Create the Clp interface library.
add_subdirectory(cpp)

# Create the Clp interface csharp library.
add_subdirectory(csharp)
