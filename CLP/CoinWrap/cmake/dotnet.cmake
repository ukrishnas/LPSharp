find_program(DOTNET_EXECUTABLE NAMES dotnet)
if(NOT DOTNET_EXECUTABLE)
  message(FATAL_ERROR "Dotnet program not found")
else()
  message(STATUS "Using dotnet program ${DOTNET_EXECUTABLE}")
endif()

# Variable names for package creation. Note that the first two variables are
# used to substitute variables in csproj files.
set(COINWRAP_PACKAGE CoinOr.Clp)
set(COINWRAP_CSHARP_DIR ${PROJECT_BINARY_DIR}/csharp)
set(COINWRAP_PACKAGES_DIR ${PROJECT_BINARY_DIR}/dotnet/packages)
if(WIN32)
  set(RUNTIME_IDENTIFIER win-x64)
else()
  message(FATAL_ERROR "Unsupported system !")
endif()
set(COINWRAP_PROJECT ${COINWRAP_PACKAGE})
set(COINWRAP_NATIVE_PROJECT ${COINWRAP_PACKAGE}.runtime.${RUNTIME_IDENTIFIER})
set(COINWRAP_PATH ${PROJECT_BINARY_DIR}/dotnet/${COINWRAP_PROJECT})
set(COINWRAP_NATIVE_PATH ${PROJECT_BINARY_DIR}/dotnet/${COINWRAP_NATIVE_PROJECT})

# Copy directory level build properties used by all csproj files.
configure_file(
  ${PROJECT_SOURCE_DIR}/csharp/Directory.Build.props.in
  ${PROJECT_BINARY_DIR}/dotnet/Directory.Build.props
  @ONLY
)

# This section creates the native package for coinwrap_cpp.dll. First create
# the csproj file in two steps; configure file handles variable names and file
# generate handles generator expressions. Build and pack the DLL into a nuget
# package using dotnet. 
message(STATUS "Package ${COINWRAP_NATIVE_PROJECT} ${PROJECT_VERSION}")
configure_file(
  ${PROJECT_SOURCE_DIR}/csharp/${COINWRAP_PACKAGE}.runtime.csproj.in
  ${COINWRAP_NATIVE_PATH}/${COINWRAP_NATIVE_PROJECT}.csproj.in
  @ONLY
)
file(GENERATE
  OUTPUT ${COINWRAP_NATIVE_PATH}/$<CONFIG>/${COINWRAP_NATIVE_PROJECT}.csproj.in
  INPUT ${COINWRAP_NATIVE_PATH}/${COINWRAP_NATIVE_PROJECT}.csproj.in
)
add_custom_command(
  OUTPUT ${COINWRAP_NATIVE_PATH}/${COINWRAP_NATIVE_PROJECT}.csproj
  DEPENDS ${COINWRAP_NATIVE_PATH}/$<CONFIG>/${COINWRAP_NATIVE_PROJECT}.csproj.in
  COMMAND ${CMAKE_COMMAND} -E copy ./$<CONFIG>/${COINWRAP_NATIVE_PROJECT}.csproj.in ${COINWRAP_NATIVE_PROJECT}.csproj
  WORKING_DIRECTORY ${COINWRAP_NATIVE_PATH}
)
add_custom_target(coinwrap_native_package
  DEPENDS
    ${COINWRAP_NATIVE_PROJECT}/${COINWRAP_NATIVE_PROJECT}.csproj
    coinwrap_cpp
  COMMAND ${CMAKE_COMMAND} -E make_directory packages
  COMMAND ${DOTNET_EXECUTABLE} build -c Release /p:Platform=x64 ${COINWRAP_NATIVE_PROJECT}/${COINWRAP_NATIVE_PROJECT}.csproj
  COMMAND ${DOTNET_EXECUTABLE} pack -c Release ${COINWRAP_NATIVE_PROJECT}/${COINWRAP_NATIVE_PROJECT}.csproj
  BYPRODUCTS
    dotnet/${COINWRAP_NATIVE_PROJECT}/bin
    dotnet/${COINWRAP_NATIVE_PROJECT}/obj
  WORKING_DIRECTORY dotnet
)
add_dependencies(coinwrap_native_package coinwrap_cpp)

# This section creates the .Net csharp package. First create the csproj file in
# two steps; configure file handles variable names and file generate handles
# generator expressions. Build and pack the DLL into a nuget package using dotnet.
message(STATUS "Package ${COINWRAP_PROJECT} ${PROJECT_VERSION}")
configure_file(
  ${PROJECT_SOURCE_DIR}/csharp/${COINWRAP_PACKAGE}.csproj.in
  ${COINWRAP_PATH}/${COINWRAP_PROJECT}.csproj.in
  @ONLY
)
file(GENERATE
  OUTPUT ${COINWRAP_PATH}/$<CONFIG>/${COINWRAP_PROJECT}.csproj.in
  INPUT ${COINWRAP_PATH}/${COINWRAP_PROJECT}.csproj.in
)
add_custom_command(
  OUTPUT ${COINWRAP_PATH}/${COINWRAP_PROJECT}.csproj
  DEPENDS ${COINWRAP_PATH}/$<CONFIG>/${COINWRAP_PROJECT}.csproj.in
  COMMAND ${CMAKE_COMMAND} -E copy ./$<CONFIG>/${COINWRAP_PROJECT}.csproj.in ${COINWRAP_PROJECT}.csproj
  WORKING_DIRECTORY ${COINWRAP_PATH}
)
add_custom_target(coinwrap_package ALL
  DEPENDS ${COINWRAP_PROJECT}/${COINWRAP_PROJECT}.csproj
  COMMAND ${CMAKE_COMMAND} -E make_directory packages
  COMMAND ${DOTNET_EXECUTABLE} build -c Release /p:Platform=x64 ${COINWRAP_PROJECT}/${COINWRAP_PROJECT}.csproj
  COMMAND ${DOTNET_EXECUTABLE} pack -c Release ${COINWRAP_PROJECT}/${COINWRAP_PROJECT}.csproj
  BYPRODUCTS
    dotnet/${COINWRAP_PROJECT}/bin
    dotnet/${COINWRAP_PROJECT}/obj
    dotnet/packages
  WORKING_DIRECTORY dotnet
)
add_dependencies(coinwrap_package coinwrap_native_package)

