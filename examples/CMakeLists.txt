cmake_minimum_required(VERSION 3.15)
project(example LANGUAGES CXX)

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
# libraries, and link options. We enable whole program optimization at link time
# since libClp is compiled with it.
add_library(clp_pseudo INTERFACE)
set_target_properties(clp_pseudo PROPERTIES
  CXX_STANDARD 17
  CXX_STANDARD_REQUIRED ON
  CXX_EXTENSIONS OFF
  POSITION_INDEPENDENT_CODE ON
)
target_link_options(clp_pseudo INTERFACE "/LTCG")
target_include_directories(clp_pseudo INTERFACE
  ${PROJECT_SOURCE_DIR}/../../Clp/src
  ${PROJECT_SOURCE_DIR}/../../CoinUtils/src
  ${PROJECT_SOURCE_DIR}/../../BuildTools/headers
)
target_link_libraries(clp_pseudo INTERFACE
  LIB_CLP
  LIB_COINUTILS
)

# Set the source file to the example you wish to build.
set(EXAMPLE_SRC addColumns.cpp)
get_filename_component(EXAMPLE_NAME ${EXAMPLE_SRC} NAME_WE)
add_executable(${EXAMPLE_NAME} ${EXAMPLE_SRC})
target_link_libraries(${EXAMPLE_NAME} clp_pseudo)
