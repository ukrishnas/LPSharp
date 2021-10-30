set_property(SOURCE clp_interface.i PROPERTY CPLUSPLUS ON)
set_property(SOURCE clp_interface.i PROPERTY SWIG_MODULE_NAME coinwrap)
set_property(SOURCE clp_interface.i PROPERTY COMPILE_OPTIONS
    -dllimport coinwrap
    -dllimport libClp
    -dllimport libCoinUtils)

set(NAME csharp_coinwrap)

swig_add_library(${NAME}
    TYPE OBJECT
    LANGUAGE csharp
    OUTPUT_DIR ${PROJECT_BINARY_DIR}/dotnet/${PROJECT_NAME}
    SOURCES clp_interface.i
)

set_target_properties(${NAME} PROPERTIES
    SWIG_USE_TARGET_INCLUDE_DIRECTORIES ON
    POSITION_INDEPENDENT_CODE ON
)
target_link_libraries(${NAME} PRIVATE
  ${PROJECT_SOURCE_DIR}/../../Clp/x64/Release/libClp.lib
  ${PROJECT_SOURCE_DIR}/../../CoinUtils/x64/Release/libCoinUtils.lib
)