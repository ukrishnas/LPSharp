# Set interface properties.
set_property(SOURCE clp_interface.i PROPERTY CPLUSPLUS ON)
set_property(SOURCE clp_interface.i PROPERTY SWIG_MODULE_NAME clpinterface-csharp)
set_property(SOURCE clp_interface.i PROPERTY COMPILE_OPTIONS
  -namespace CoinOr.Clp
  -dllimport ClpInterface-cpp
  -dllimport libClp
  -dllimport libCoinUtils)

set(NAME ClpInterface-csharp)

swig_add_library(${NAME}
    TYPE OBJECT
    LANGUAGE csharp
    OUTPUT_DIR ${PROJECT_BINARY_DIR}/csharp/${PROJECT_NAME}
    SOURCES clp_interface.i
)

set_target_properties(${NAME} PROPERTIES
    SWIG_USE_TARGET_INCLUDE_DIRECTORIES ON
    POSITION_INDEPENDENT_CODE ON
)
target_link_libraries(${NAME} PRIVATE ClpInterface-cpp)
