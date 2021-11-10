cmake_minimum_required(VERSION 3.15)

# Set policy that UseSWIG generates standard target names.
if(POLICY CMP0078)
  cmake_policy(SET CMP0078 NEW)
endif()

# Set policy that UseSWIG passes SWIG_MODULE_NAME property via -module flag to SWIG.
if(POLICY CMP0086)
  cmake_policy(SET CMP0086 NEW)
endif()

# UseSWIG generates library with default naming conventions for CSharp language.
# This was introduced in CMake version 3.21 and hence this policy is set to OLD
# for now.
if(POLICY CMP0122)
  cmake_policy(SET CMP0122 OLD)
endif()

# Define project after setting policies.
project(coinwrap)

# Build the Clp interface. This is the interface that will wrapped using SWIG.
add_subdirectory(cpp)

# Define the native library.
add_library(coinwrap_cpp SHARED "")
set_target_properties(coinwrap_cpp PROPERTIES
  PREFIX ""
  POSITION_INDEPENDENT_CODE ON
)

# Fetch SWIG from public location if this is the first time and initialize SWIG.
include(FetchContent)
set(FETCHCONTENT_QUIET ON)
set(FETCHCONTENT_UPDATES_DISCONNECTED ON)
set(BUILD_SWIG ON)
if (BUILD_SWIG)
  message(CHECK_START "Fetching SWIG")
  list(APPEND CMAKE_MESSAGE_INDENT "  ")
  FetchContent_Declare(
    SWIG
    URL    "http://prdownloads.sourceforge.net/swig/swigwin-4.0.2.zip"
  )
  FetchContent_MakeAvailable(SWIG)
  set(
    SWIG_DIR "${swig_SOURCE_DIR}/Lib"
    CACHE INTERNAL "SWIG directory" FORCE
  )
  set(
    SWIG_EXECUTABLE "${swig_SOURCE_DIR}/swig.exe"
    CACHE INTERNAL "SWIG executable location" FORCE
  )
  message(CHECK_PASS "fetched")
  list(POP_BACK CMAKE_MESSAGE_INDENT)

  set(CMAKE_SWIG_FLAGS)
  find_package(SWIG REQUIRED)
  if (NOT SWIG_FOUND)
    message(FATAL_ERROR "SWIG not found after fetch")
  endif()
endif()
include(UseSWIG)

# Generate C++ and C# wrapper code and import the generated C++ wrapper with the
# Clp libraries into a new coinwrap_cpp.dll. At the end of this step, this DLL
# should be present in the build tree.
list(APPEND CMAKE_SWIG_FLAGS "-I${PROJECT_SOURCE_DIR}/cpp")
add_subdirectory(csharp)
target_link_libraries(coinwrap_cpp coinwrap_csharp)

return()

# This section is to compile the C# wrapper code and generate nuget packages.
find_program(DOTNET_EXECUTABLE NAMES dotnet)
if(NOT DOTNET_EXECUTABLE)
  message(FATAL_ERROR "Check for dotnet Program: not found")
else()
  message(STATUS "Found dotnet Program: ${DOTNET_EXECUTABLE}")
endif()

set(COINWRAP_PACKAGE CoinOr.Clp)
set(COINWRAP_PACKAGE_DIR "${PROJECT_BINARY_DIR}/csharp/packages")
if(WIN32)
  set(RUNTIME_IDENTIFIER win-x64)
else()
  message(FATAL_ERROR "Unsupported system !")
endif()
set(COINWRAP_NATIVE_PACKAGE ${COINWRAP_PACKAGE}.runtime.${RUNTIME_IDENTIFIER})
set(COINWRAP_PROJECT ${COINWRAP_PACKAGE})
