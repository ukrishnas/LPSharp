cmake_minimum_required(VERSION 3.15)
project(coinwrap)
set(NAME ${PROJECT_NAME})

# SWIG: use standard target name.
if(POLICY CMP0078)
  cmake_policy(SET CMP0078 NEW)
endif()

# SWIG: use SWIG_MODULE_NAME property.
if(POLICY CMP0086)
  cmake_policy(SET CMP0086 NEW)
endif()

# SWIG: Use library name conventions for CSharp language.
if(POLICY CMP0122)
  cmake_policy(SET CMP0122 OLD)
endif()

# External project for fetching SWIG binary.
include(ExternalProject)
ExternalProject_Add(SWIG_project
  TMP_DIR "${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}/tmp"
  STAMP_DIR "${CMAKE_CURRENT_BINARY_DIR}PROJECT_NAME}/stamp"
  DOWNLOAD_DIR "${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}/download"
  SOURCE_DIR "${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}/source"
  BUILD_IN_SOURCE 1

  URL "http://prdownloads.sourceforge.net/swig/swigwin-4.0.2.zip"
  LOG_DOWNLOAD TRUE
  UPDATE_COMMAND ""
  LOG_UPDATE TRUE
  PATCH_COMMAND ""
  LOG_PATCH TRUE
  CONFIGURE_COMMAND ""
  LOG_CONFIGURE TRUE
  BUILD_COMMAND ""
  LOG_BUILD TRUE
  INSTALL_COMMAND ""
  LOG_INSTALL TRUE
  TEST_COMMAND ""
  LOG_TEST TRUE
)

# If configured, fetch and build SWIG.
if(BUILD_SWIG)
  message(CHECK_START "Fetching SWIG")
  list(APPEND CMAKE_MESSAGE_INDENT "  ")

  execute_process(
    COMMAND ${CMAKE_COMMAND} -H. -Bproject_build -G "${CMAKE_GENERATOR}"
    RESULT_VARIABLE result
    WORKING_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}/SWIG
  )
  if(result)
    message(FATAL_ERROR "CMake step for SWIG failed: ${result}")
  endif()

  execute_process(
    COMMAND ${CMAKE_COMMAND} --build project_build --config ${CMAKE_BUILD_TYPE}
    RESULT_VARIABLE result
    WORKING_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}/SWIG
  )
  if(result)
    message(FATAL_ERROR "Build step for SWIG failed: ${result}")
  endif()

  set(SWIG_EXECUTABLE
    ${CMAKE_CURRENT_BINARY_DIR}/SWIG/source/swig.exe
    CACHE INTERNAL "swig.exe location" FORCE)
  list(POP_BACK CMAKE_MESSAGE_INDENT)
  message(CHECK_PASS "fetched")
endif()

set(CMAKE_SWIG_FLAGS)
find_package(SWIG REQUIRED)
include(UseSWIG)

if(UNIX AND NOT APPLE)
  list(APPEND CMAKE_SWIG_FLAGS "-DSWIGWORDSIZE64")
endif()

# Find dotnet cli
find_program(DOTNET_EXECUTABLE NAMES dotnet)
if(NOT DOTNET_EXECUTABLE)
  message(FATAL_ERROR "Check for dotnet Program: not found")
else()
  message(STATUS "Found dotnet Program: ${DOTNET_EXECUTABLE}")
endif()

# Create the Clp interface cpp library.
add_subdirectory(cpp)

# Create the Clp interface csharp library.
list(APPEND CMAKE_SWIG_FLAGS "-I${PROJECT_SOURCE_DIR}/cpp")
add_subdirectory(csharp)

# Needed for nuget packages.
set(DOTNET_PACKAGE CoinOr.Clp)
set(DOTNET_PACKAGES_DIR "${PROJECT_BINARY_DIR}/dotnet/packages")
if(WIN32)
  set(RUNTIME_IDENTIFIER win-x64)
else()
  message(FATAL_ERROR "Unsupported system !")
endif()
set(DOTNET_NATIVE_PROJECT ${DOTNET_PACKAGE}.runtime.${RUNTIME_IDENTIFIER})
set(DOTNET_PROJECT ${DOTNET_PACKAGE})

