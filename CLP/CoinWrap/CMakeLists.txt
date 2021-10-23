cmake_minimum_required(VERSION 3.15)
project(CoinWrap LANGUAGES CXX)

add_executable(coinwrap
  rowColumn.cpp
)

target_include_directories(coinwrap PUBLIC
  ${PROJECT_SOURCE_DIR}/../Clp/src
  ${PROJECT_SOURCE_DIR}/../CoinUtils/src
  ${PROJECT_SOURCE_DIR}/../BuildTools/headers
)

target_link_libraries(coinwrap
  ${PROJECT_SOURCE_DIR}/../Clp/x64/Release/libClp.lib
  ${PROJECT_SOURCE_DIR}/../CoinUtils/x64/Release/libCoinUtils.lib
)