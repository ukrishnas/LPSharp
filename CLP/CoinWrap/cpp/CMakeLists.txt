cmake_minimum_required(VERSION 3.15)
project(clpinterface_cpp LANGUAGES CXX)

# Set some global defaults that are used to initialize target properties when
# the targets are created.
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)
set(CMAKE_CXX_EXTENSIONS False)

# Add external dependencies for libClp and libCoinUtils. The import locations
# are where they are placed by Visual Studio during build.
add_library(LIB_CLP SHARED IMPORTED)
set_target_properties(LIB_CLP PROPERTIES
  IMPORTED_IMPLIB ${PROJECT_SOURCE_DIR}/../../x64/Release/libClp.lib
)
add_library(LIB_COINUTILS SHARED IMPORTED)
set_target_properties(LIB_COINUTILS PROPERTIES
  IMPORTED_IMPLIB ${PROJECT_SOURCE_DIR}/../../x64/Release/libCoinUtils.lib
)

# Create a CMake pseudo target for properties, include directories, link
# libraries, and link options. We enable whole program optimization with /LTCG
# at link time since libClp is compiled with it.
add_library(clp_pseudo INTERFACE)
target_include_directories(clp_pseudo INTERFACE
  ${PROJECT_SOURCE_DIR}
  ${PROJECT_SOURCE_DIR}/../../Clp/src
  ${PROJECT_SOURCE_DIR}/../../CoinUtils/src
  ${PROJECT_SOURCE_DIR}/../../BuildTools/headers
)
target_link_options(clp_pseudo INTERFACE "/LTCG")
target_link_libraries(clp_pseudo INTERFACE
  LIB_CLP
  LIB_COINUTILS
)

# Create the Clp interface library.
set(LIB_CLP_INTERFACE clpinterface_cpp)
add_library(${LIB_CLP_INTERFACE} clp_interface.cc)
set_target_properties(${LIB_CLP_INTERFACE} PROPERTIES
  POSITION_INDEPENDENT_CODE ON
)
target_link_libraries(${LIB_CLP_INTERFACE} clp_pseudo)

# Create the Clp interface executable.
set(EXE_CLP_INTERFACE coinwrap)
add_executable(${EXE_CLP_INTERFACE} coin_wrap.cc)
target_link_libraries(${EXE_CLP_INTERFACE}
  ${LIB_CLP_INTERFACE}
  clp_pseudo
)

# Create an alias for the project. Alias targets can be used as linkable targets
# and as targets to read properties from.
add_library(coinwrap::ClpInterface ALIAS ${PROJECT_NAME})

include(GnuInstallDirs)
include(CTest)

# Builds and adds a unit test.
function(add_coinwrap_test FILE_NAME)
  message(STATUS "Configuring test ${FILE_NAME}")
  get_filename_component(TEST_NAME ${FILE_NAME} NAME_WE)
  list(APPEND _TESTS ${TEST_NAME})

  add_executable(${TEST_NAME} ${FILE_NAME})
  target_link_libraries(${TEST_NAME}
    ${LIB_CLP_INTERFACE}
    clp_pseudo
  )
  add_test(NAME ${TEST_NAME} COMMAND ${TEST_NAME})
endfunction()

# Build and run unit tests.
if(BUILD_TESTING)
  # Copy test data files.
  configure_file(${PROJECT_SOURCE_DIR}/tests/80bau38.mps ${PROJECT_BINARY_DIR} COPYONLY)

  # Build tests.
  set(_TESTS)
  list(APPEND _TESTS
    ${PROJECT_SOURCE_DIR}/tests/clp_build_model_test.cc
    ${PROJECT_SOURCE_DIR}/tests/clp_solve_model_test.cc
  )
  foreach(filename ${_TESTS})
    add_coinwrap_test(${filename})
  endforeach()

  # Run tests after they are built.
  add_custom_target(RunTests ALL
    COMMAND ${CMAKE_CTEST_COMMAND} -test_dir . -C $<IF:$<CONFIG:Release>,Release,Debug>
    WORKING_DIRECTORY ${PROJECT_BINARY_DIR}
  )
  foreach(filename ${_TESTS})
    get_filename_component(test_name ${filename} NAME_WE)
    add_dependencies(RunTests ${test_name})
  endforeach()
endif()