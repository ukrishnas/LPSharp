set_property(SOURCE clp_interface.i PROPERTY CPLUSPLUS ON)
set_property(SOURCE clp_interface.i PROPERTY COMPILE_OPTIONS
  -namespace CoinOr.Clp
  -dllimport coinwrap_cpp
)
swig_add_library(coinwrap_csharp
  TYPE OBJECT
  LANGUAGE csharp
  OUTPUT_DIR ${PROJECT_BINARY_DIR}/csharp
  SOURCES clp_interface.i
)
set_target_properties(coinwrap_csharp PROPERTIES
  SWIG_USE_TARGET_INCLUDE_DIRECTORIES ON
  POSITION_INDEPENDENT_CODE ON
)
target_link_libraries(coinwrap_csharp PRIVATE coinwrap::ClpInterface)
