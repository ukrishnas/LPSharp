# This file is just an orchestration
cmake_minimum_required(VERSION 3.15)
list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/cmake")

# Apple: Don't modify install_name when touching RPATH.
if(POLICY CMP0068)
  cmake_policy(SET CMP0068 NEW)
endif()

# Define file(GENERATE) behavior for relative paths.
if(POLICY CMP0070)
  cmake_policy(SET CMP0070 NEW)
endif()

# target_sources: use absolute path for INTERFACE_SOURCES.
if(POLICY CMP0076)
  cmake_policy(SET CMP0076 NEW)
endif()

# option() honors normal variables.
if(POLICY CMP0077)
  cmake_policy(SET CMP0077 NEW)
endif()

# SWIG: use standard target name.
if(POLICY CMP0078)
  cmake_policy(SET CMP0078 NEW)
endif()

# SWIG: use SWIG_MODULE_NAME property.
if(POLICY CMP0086)
  cmake_policy(SET CMP0086 NEW)
endif()

# MSVC runtime library flags are selected by an abstraction.
if(POLICY CMP0091)
  cmake_policy(SET CMP0091 NEW)
endif()

# SWIG: Use library name conventions for CSharp language.
if(POLICY CMP0122)
  cmake_policy(SET CMP0122 OLD)
endif()

include(utils)
set_version(VERSION)

project(ortools VERSION ${VERSION} LANGUAGES CXX)
set(PROJECT_NAMESPACE ortools)
message(STATUS "${PROJECT_NAME} version: ${PROJECT_VERSION}")

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Default build type to be Release
if(NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE "Release" CACHE STRING
    "Choose the type of build, options are: Debug Release RelWithDebInfo MinSizeRel. (default: Release)"
    FORCE)
endif()

# Layout build directory like install directory
include(GNUInstallDirs)
if(UNIX)
  option(BUILD_SHARED_LIBS "Build shared libraries (.so or .dyld)." ON)
  set(CMAKE_BUILD_WITH_INSTALL_RPATH TRUE)
  set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/${CMAKE_INSTALL_LIBDIR})
  set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/${CMAKE_INSTALL_LIBDIR})
  set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/${CMAKE_INSTALL_BINDIR})
  # For multi-config build system (e.g. xcode)
  foreach(OUTPUTCONFIG IN LISTS CMAKE_CONFIGURATION_TYPES)
    string(TOUPPER ${OUTPUTCONFIG} OUTPUTCONFIG)
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${CMAKE_BINARY_DIR}/${OUTPUTCONFIG}/${CMAKE_INSTALL_LIBDIR})
    set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${CMAKE_BINARY_DIR}/${OUTPUTCONFIG}/${CMAKE_INSTALL_LIBDIR})
    set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${CMAKE_BINARY_DIR}/${OUTPUTCONFIG}/${CMAKE_INSTALL_BINDIR})
  endforeach()
else()
  # Currently only support static build for windows
  option(BUILD_SHARED_LIBS "Build shared libraries (.dll)." OFF)
  set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/${CMAKE_INSTALL_BINDIR})
  set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/${CMAKE_INSTALL_BINDIR})
  set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/${CMAKE_INSTALL_BINDIR})
  # For multi-config builds (e.g. msvc)
  foreach(OUTPUTCONFIG IN LISTS CMAKE_CONFIGURATION_TYPES)
    string(TOUPPER ${OUTPUTCONFIG} OUTPUTCONFIG)
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${CMAKE_BINARY_DIR}/${OUTPUTCONFIG}/${CMAKE_INSTALL_BINDIR})
    set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${CMAKE_BINARY_DIR}/${OUTPUTCONFIG}/${CMAKE_INSTALL_BINDIR})
    set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_${OUTPUTCONFIG} ${CMAKE_BINARY_DIR}/${OUTPUTCONFIG}/${CMAKE_INSTALL_BINDIR})
  endforeach()
endif()

# By default build the C++ and dotnet libraries.
option(BUILD_CXX "Build C++ library" ON)
message(STATUS "Build C++ library: ${BUILD_CXX}")
option(BUILD_DOTNET "Build .NET Library" ON)
message(STATUS "Build .Net: ${BUILD_DOTNET}")

# If wrapper are built, we need to have the install rpath in BINARY_DIR to package.
if(BUILD_DOTNET)
  set(CMAKE_BUILD_WITH_INSTALL_RPATH TRUE)
endif()

include(CMakeDependentOption)

CMAKE_DEPENDENT_OPTION(BUILD_GLOP "Build GLOP standalone" ON "NOT BUILD_CXX" OFF)
message(STATUS "Build standalone Glop: ${BUILD_GLOP}")

# By default all dependencies are NOT built (i.e. BUILD_DEPS=OFF),
# But if building any wrappers like dotnet, then BUILD_DEPS=ON.
if(BUILD_DOTNET)
  option(BUILD_DEPS "Build all dependencies" ON)
else()
  option(BUILD_DEPS "Build all dependencies" OFF)
endif()
message(STATUS "Build all dependencies: ${BUILD_DEPS}")
# Install built dependencies if any.
option(INSTALL_BUILD_DEPS "Install build all dependencies" ON)

# IF BUILD_DEPS=ON THEN force all BUILD_*=ON and do not give this option to the user.
CMAKE_DEPENDENT_OPTION(BUILD_ZLIB "Build the ZLIB dependency Library" ON "NOT BUILD_DEPS" ON)
message(STATUS "Build ZLIB: ${BUILD_ZLIB}")

CMAKE_DEPENDENT_OPTION(BUILD_absl "Build the abseil-cpp dependency Library" ON "NOT BUILD_DEPS" ON)
message(STATUS "Build abseil-cpp: ${BUILD_absl}")

CMAKE_DEPENDENT_OPTION(BUILD_Protobuf "Build the Protobuf dependency Library" ON "NOT BUILD_DEPS" ON)
message(STATUS "Build protobuf: ${BUILD_Protobuf}")

# Build Needed dependencies
add_subdirectory(cmake/dependencies dependencies)
list(APPEND CMAKE_PREFIX_PATH ${CMAKE_CURRENT_BINARY_DIR}/dependencies/install)

include(CTest)

# Basic type
include(CMakePushCheckState)
cmake_push_check_state(RESET)
set(CMAKE_EXTRA_INCLUDE_FILES "cstdint")
include(CheckTypeSize)
check_type_size("long" SIZEOF_LONG LANGUAGE CXX)
message(STATUS "Found long size: ${SIZEOF_LONG}")
check_type_size("long long" SIZEOF_LONG_LONG LANGUAGE CXX)
message(STATUS "Found long long size: ${SIZEOF_LONG_LONG}")
check_type_size("int64_t" SIZEOF_INT64_T LANGUAGE CXX)
message(STATUS "Found int64_t size: ${SIZEOF_INT64_T}")

check_type_size("unsigned long" SIZEOF_ULONG LANGUAGE CXX)
message(STATUS "Found unsigned long size: ${SIZEOF_ULONG}")
check_type_size("unsigned long long" SIZEOF_ULONG_LONG LANGUAGE CXX)
message(STATUS "Found unsigned long long size: ${SIZEOF_ULONG_LONG}")
check_type_size("uint64_t" SIZEOF_UINT64_T LANGUAGE CXX)
message(STATUS "Found uint64_t size: ${SIZEOF_UINT64_T}")

check_type_size("int *" SIZEOF_INT_P LANGUAGE CXX)
message(STATUS "Found int * size: ${SIZEOF_INT_P}")
cmake_pop_check_state()

if(CMAKE_CROSSCOMPILING)
  message(FATAL_ERROR "Cross-compiling is not supported.")
else()
  set(PROTOC_PRG protobuf::protoc)
endif()

include(deps)
include(cpp)
include(glop)
include(dotnet)
