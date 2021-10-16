file(GLOB _SRCS "*.h" "*.cc")

get_filename_component(MODULE_NAME ${CMAKE_CURRENT_LIST_DIR} NAME)
set(NAME ${PROJECT_NAME}_${MODULE_NAME})

add_library(${NAME} OBJECT ${_SRCS})
set_target_properties(${NAME} PROPERTIES
  CXX_STANDARD 17
  CXX_STANDARD_REQUIRED ON
  CXX_EXTENSIONS OFF
  POSITION_INDEPENDENT_CODE ON
  )
target_include_directories(${NAME} PUBLIC
  $<BUILD_INTERFACE:${PROJECT_SOURCE_DIR}>
  $<BUILD_INTERFACE:${PROJECT_BINARY_DIR}>)
target_link_libraries(${NAME} PRIVATE
  absl::memory
  absl::strings
  absl::str_format
  protobuf::libprotobuf
  ${PROJECT_NAME}::proto)
