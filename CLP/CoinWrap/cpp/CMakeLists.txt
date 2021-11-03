cmake_minimum_required(VERSION 3.15)
project(clpinterface-cpp LANGUAGES CXX)
set(NAME ClpInterface-cpp)

list(APPEND _SRCS
  clp_interface.cc
  clp_model_builder.cc
  coin_wrap.cc
)
add_executable(${NAME} ${_SRCS})

set_target_properties(${NAME} PROPERTIES
  CXX_STANDARD 17
  CXX_STANDARD_REQUIRED ON
  CXX_EXTENSIONS OFF
  POSITION_INDEPENDENT_CODE ON
)

# Enable whole program optimization at link time since libClp is compiled with it.
target_link_options(${NAME} PRIVATE "/LTCG")

target_include_directories(${NAME} PUBLIC
  ${PROJECT_SOURCE_DIR}/../../Clp/src
  ${PROJECT_SOURCE_DIR}/../../CoinUtils/src
  ${PROJECT_SOURCE_DIR}/../../BuildTools/headers
)

target_link_libraries(${NAME}
  ${PROJECT_SOURCE_DIR}/../../x64/Release/libClp.lib
  ${PROJECT_SOURCE_DIR}/../../x64/Release/libCoinUtils.lib
)

include(CTest)

# Builds and adds a unit test.
function(add_coinwrap_test FILE_NAME)
  message(STATUS "Configuring test ${FILE_NAME}: ...")
  get_filename_component(TEST_NAME ${FILE_NAME} NAME_WE)

  add_executable(${TEST_NAME} ${FILE_NAME})
  target_include_directories(${TEST_NAME} PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}../src)

  if(BUILD_TESTING)
    add_test(NAME ${TEST_NAME} COMMAND ${TEST_NAME})
  endif()
  message(STATUS "Configuring test ${FILE_NAME}: ...DONE")
endfunction()

# Add unit tests.
# add_coinwrap_test(${PROJECT_SOURCE_DIR}/tests/clp_interface_test.cc)

