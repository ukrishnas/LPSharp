include(FetchContent)
set(FETCHCONTENT_QUIET ON)
set(FETCHCONTENT_UPDATES_DISCONNECTED ON)

# Fetch SWIG from public location if this is the first time.
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

  find_package(SWIG 4.0 COMPONENTS csharp)
  if (NOT SWIG_FOUND)
    message(FATAL_ERROR "SWIG not found after fetch")
  endif()
endif()

include(UseSWIG)

set(NAME clpinterface_csharp)
set_property(SOURCE clp_interface.i PROPERTY CPLUSPLUS ON)
set_property(SOURCE clp_interface.i PROPERTY COMPILE_OPTIONS
  -namespace CoinOr.Clp
  -dllimport clpInterface-cpp
)
swig_add_library(${NAME}
  TYPE OBJECT
  LANGUAGE csharp
  OUTPUT_DIR ${PROJECT_BINARY_DIR}/csharp
  SOURCES clp_interface.i
)
target_include_directories(${NAME} PRIVATE ${CMAKE_CURRENT_SOURCE_DIR}/../cpp)
set_target_properties(${NAME} PROPERTIES
  SWIG_USE_TARGET_INCLUDE_DIRECTORIES ON
  POSITION_INDEPENDENT_CODE ON
)
target_link_libraries(${NAME} PRIVATE clpinterface_cpp)
