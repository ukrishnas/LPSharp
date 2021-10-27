cmake_minimum_required(VERSION 3.15)
project(glopsolve LANGUAGES CXX)
set(NAME ${PROJECT_NAME})

add_executable(${NAME} glop_solve.cc)

set_target_properties(${NAME} PROPERTIES
  CXX_STANDARD 17
  CXX_STANDARD_REQUIRED ON
  CXX_EXTENSIONS OFF
)

target_include_directories(${NAME} PUBLIC
  ${PROJECT_SOURCE_DIR}/..
  ${PROJECT_SOURCE_DIR}/../build2
  ${PROJECT_SOURCE_DIR}/../build2/_deps/absl-src
  ${PROJECT_SOURCE_DIR}/../build2/_deps/protobuf-src/src
)

file(GLOB _LIBS ${PROJECT_SOURCE_DIR}/../build2/RELEASE/bin/*.lib)
target_link_libraries(${NAME} PRIVATE ${_LIBS})