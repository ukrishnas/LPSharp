include(FetchContent)
set(FETCHCONTENT_QUIET ON)
set(FETCHCONTENT_UPDATES_DISCONNECTED ON)

# Fetch SWIG from public location if this is the first time. Note that the fetch
# happens at build time. If you wish to change to compile time, then use ExternalProject.
set(BUILD_SWIG ON)
if (BUILD_SWIG)
  message(CHECK_START "Fetching SWIG")
  list(APPEND CMAKE_MESSAGE_INDENT "  ")
  FetchContent_Declare(
    SWIG
    URL    "http://prdownloads.sourceforge.net/swig/swigwin-4.0.2.zip"
  )
  message(STATUS "Making content available")
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
