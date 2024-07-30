# Create the CoinUtils and Clp libraries in the parent scope so that their names
# are available to all future functions and scopes. Clp needs to link with CoinUtils.
set(LIB_COINUTILS libCoinUtils)
add_library(${LIB_COINUTILS})
set(LIB_CLP libClp)
add_library(${LIB_CLP})

# Set common definitions and options for compiling cpp code.
set(COMMON_COMPILE_DEFINITIONS)
set(COMMON_COMPILE_OPTIONS)
if(WIN32)
  list(APPEND COMMON_COMPILE_DEFINITIONS "__WIN32__")
endif()
if(MSVC)
  list(APPEND COMMON_COMPILE_DEFINITIONS
    "_CRT_SECURE_NO_WARNINGS" # Disable deprecation warnings for less secure functions.
    "_SCL_SECURE_NO_WARNINGS" # Disable warnings for unsafe methods in C++ standard library.
    $<$<CONFIG:Debug>:_DEBUG>
    $<$<CONFIG:Release>:_NDEBUG>
    $<$<CONFIG:Release>:NDEBUG>
  )
  list(APPEND COMMON_COMPILE_OPTIONS
    $<$<CONFIG:Release>:/GL> # Whole program optimization in compiler.
    $<$<CONFIG:Release>:/MP> # Multi-processor compilation.
    "/wd4146"
  )
endif()

# Set common link options. Clp vcxproj files used whole program optimization.
set(COMMON_LINK_OPTIONS
  $<$<CONFIG:Release>:/LTCG> # Whole program optimization in linker.
)

# Create a CMake pseudo target for properties, include directories, link
# libraries, and link options.
add_library(clp_pseudo INTERFACE)
target_include_directories(clp_pseudo INTERFACE
  ${PROJECT_SOURCE_DIR}/CoinUtils/src
  ${PROJECT_SOURCE_DIR}/Clp/src
  ${PROJECT_SOURCE_DIR}/CoinWrap/cpp
)
target_compile_definitions(clp_pseudo INTERFACE ${COMMON_COMPILE_DEFINITIONS})
target_compile_options(clp_pseudo INTERFACE ${COMMON_COMPILE_OPTIONS})
target_link_options(clp_pseudo INTERFACE ${COMMON_LINK_OPTIONS})
target_link_libraries(clp_pseudo INTERFACE
  ${LIB_COINUTILS}
  ${LIB_CLP}
)