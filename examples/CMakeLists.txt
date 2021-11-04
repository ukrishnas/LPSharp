cmake_minimum_required(VERSION 3.15)
project(example LANGUAGES CXX)

# Set the source file to the example you wish to build. 
list(APPEND _SRCS
   addColumns.cpp
#  addRows.cpp
#  rowColumn.cpp
)
add_executable(${PROJECT_NAME} ${_SRCS})

set_target_properties(${PROJECT_NAME} PROPERTIES
  CXX_STANDARD 17
  CXX_STANDARD_REQUIRED ON
  CXX_EXTENSIONS OFF
  POSITION_INDEPENDENT_CODE ON
)

# Enable whole program optimization at link time since libClp is compiled with it.
target_link_options(${PROJECT_NAME} PRIVATE "/LTCG")

target_include_directories(${PROJECT_NAME} PUBLIC
  ${PROJECT_SOURCE_DIR}/../src
  ${PROJECT_SOURCE_DIR}/../../CoinUtils/src
  ${PROJECT_SOURCE_DIR}/../../BuildTools/headers
)

# This project assumes that libClp and libCoinUtils are already built. Please
# build the library for the corresponding configuration (release or debug) using
# Visual Studio and Clp.sln in the parent folder.
add_library(LIB_CLP SHARED IMPORTED)
set_target_properties(LIB_CLP PROPERTIES
  IMPORTED_IMPLIB ${PROJECT_SOURCE_DIR}/../../x64/Release/libClp.lib
)
add_library(LIB_COINUTILS SHARED IMPORTED)
set_target_properties(LIB_COINUTILS PROPERTIES
  IMPORTED_IMPLIB ${PROJECT_SOURCE_DIR}/../../x64/Release/libCoinUtils.lib
)

# Link example with libClp and libCoinUtils.
target_link_libraries(${PROJECT_NAME}
  LIB_CLP
  LIB_COINUTILS
)
