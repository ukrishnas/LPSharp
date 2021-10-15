# ##############################################################################
# SWIG (WIN32)
# ##############################################################################
if(WIN32
   AND BUILD_DOTNET)
  message(CHECK_START "Fetching SWIG")
  list(APPEND CMAKE_MESSAGE_INDENT "  ")
  configure_file(${CMAKE_CURRENT_SOURCE_DIR}/SWIG.CMakeLists.txt.in
                 ${CMAKE_CURRENT_BINARY_DIR}/SWIG/CMakeLists.txt @ONLY)

  execute_process(
    COMMAND ${CMAKE_COMMAND} -H. -Bproject_build -G "${CMAKE_GENERATOR}"
    RESULT_VARIABLE result
    WORKING_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}/SWIG)
  if(result)
    message(FATAL_ERROR "CMake step for SWIG failed: ${result}")
  endif()

  execute_process(
    COMMAND ${CMAKE_COMMAND} --build project_build --config ${CMAKE_BUILD_TYPE}
    RESULT_VARIABLE result
    WORKING_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}/SWIG)
  if(result)
    message(FATAL_ERROR "Build step for SWIG failed: ${result}")
  endif()

  set(SWIG_EXECUTABLE
      ${CMAKE_CURRENT_BINARY_DIR}/SWIG/source/swig.exe
      CACHE INTERNAL "swig.exe location" FORCE)
  list(POP_BACK CMAKE_MESSAGE_INDENT)
  message(CHECK_PASS "fetched")
endif()

include(FetchContent)
set(FETCHCONTENT_QUIET OFF)
set(FETCHCONTENT_UPDATES_DISCONNECTED ON)
set(BUILD_SHARED_LIBS OFF)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)
set(BUILD_TESTING OFF)

# ##############################################################################
# ZLIB
# ##############################################################################
if(BUILD_ZLIB)
  message(CHECK_START "Fetching ZLIB from dependencies")
  list(APPEND CMAKE_MESSAGE_INDENT "  ")
  FetchContent_Declare(
    zlib
    GIT_REPOSITORY "https://github.com/madler/ZLIB.git"
    GIT_TAG "v1.2.11"
    PATCH_COMMAND git apply "${CMAKE_CURRENT_LIST_DIR}/../../patches/ZLIB.patch")
  FetchContent_MakeAvailable(zlib)
  list(POP_BACK CMAKE_MESSAGE_INDENT)
  message(CHECK_PASS "fetched")
endif()

# ##############################################################################
# ABSEIL-CPP
# ##############################################################################
if(BUILD_absl)
  message(CHECK_START "Fetching Abseil-cpp from dependencies")
  list(APPEND CMAKE_MESSAGE_INDENT "  ")
  set(ABSL_ENABLE_INSTALL ON)
  FetchContent_Declare(
    absl
    GIT_REPOSITORY "https://github.com/abseil/abseil-cpp.git"
    GIT_TAG "20210324.2"
    PATCH_COMMAND git apply "${CMAKE_CURRENT_LIST_DIR}/../../patches/abseil-cpp-20210324.2.patch")
  FetchContent_MakeAvailable(absl)
  list(POP_BACK CMAKE_MESSAGE_INDENT)
  message(CHECK_PASS "fetched")
endif()

# ##############################################################################
# Protobuf
# ##############################################################################
if(BUILD_Protobuf)
  message(CHECK_START "Fetching Protobuf from dependencies")
  list(APPEND CMAKE_MESSAGE_INDENT "  ")
  set(protobuf_BUILD_TESTS OFF)
  set(protobuf_BUILD_EXPORT OFF)
  set(protobuf_MSVC_STATIC_RUNTIME OFF)
  # FetchContent_Declare(SOURCE_SUBDIR) was introduced in 3.18
  if(${CMAKE_VERSION} VERSION_GREATER_EQUAL "3.18")
    FetchContent_Declare(
      protobuf
      GIT_REPOSITORY "https://github.com/protocolbuffers/protobuf.git"
      GIT_TAG "v3.18.0"
      GIT_SUBMODULES ""
      PATCH_COMMAND git apply "${CMAKE_CURRENT_LIST_DIR}/../../patches/protobuf-v3.18.0.patch"
      SOURCE_SUBDIR cmake)
    FetchContent_MakeAvailable(protobuf)
  else()
    fetch_git_dependency(
      NAME Protobuf
      REPOSITORY "https://github.com/protocolbuffers/protobuf.git"
      TAG "v3.18.0"
      PATCH_COMMAND "git apply \"${CMAKE_CURRENT_LIST_DIR}/../../patches/protobuf-v3.18.0.patch\""
      SOURCE_SUBDIR cmake
      )
  endif()
  list(POP_BACK CMAKE_MESSAGE_INDENT)
  message(CHECK_PASS "fetched")
endif()
