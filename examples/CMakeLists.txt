# Set the source file to the example you wish to build.
set(EXAMPLE_SRC addRows.cpp)
get_filename_component(EXAMPLE_NAME ${EXAMPLE_SRC} NAME_WE)
add_executable(${EXAMPLE_NAME} ${EXAMPLE_SRC})
target_link_libraries(${EXAMPLE_NAME} clp_pseudo)
target_link_options(${EXAMPLE_NAME} PRIVATE ${COMMON_LINK_OPTIONS})