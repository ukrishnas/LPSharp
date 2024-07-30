# Create the Clp interface library.
set(LIB_CLP_INTERFACE clpinterface_cpp)
add_library(${LIB_CLP_INTERFACE} clp_interface.cc clp_interface.h)
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
add_library(coinwrap::ClpInterface ALIAS ${LIB_CLP_INTERFACE})

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
  # The test binary directory is the location where CTest will be run. Copy test
  # data files to the binary directory.
  set(TEST_BINARY_DIR ${PROJECT_BINARY_DIR}/cpp)
  configure_file(${CMAKE_CURRENT_SOURCE_DIR}/tests/80bau38.mps ${TEST_BINARY_DIR} COPYONLY)

  # Build tests.
  set(_TESTS)
  list(APPEND _TESTS
    ${CMAKE_CURRENT_SOURCE_DIR}/tests/clp_build_model_test.cc
    ${CMAKE_CURRENT_SOURCE_DIR}/tests/clp_solve_model_test.cc
  )
  foreach(filename ${_TESTS})
    add_coinwrap_test(${filename})
  endforeach()

  # Run tests after they are built.
  add_custom_target(RunTests ALL
    COMMAND ${CMAKE_CTEST_COMMAND} -C $<CONFIG>
    WORKING_DIRECTORY ${TEST_BINARY_DIR}
  )
  foreach(filename ${_TESTS})
    get_filename_component(test_name ${filename} NAME_WE)
    add_dependencies(RunTests ${test_name})
  endforeach()
endif()