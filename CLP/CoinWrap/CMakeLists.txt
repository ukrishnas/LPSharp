# Set policy that UseSWIG generates standard target names.
if(POLICY CMP0078)
  cmake_policy(SET CMP0078 NEW)
endif()

# Set policy that UseSWIG passes SWIG_MODULE_NAME property via -module flag to SWIG.
if(POLICY CMP0086)
  cmake_policy(SET CMP0086 NEW)
endif()

# UseSWIG generates library with default naming conventions for CSharp language.
# This was introduced in CMake version 3.21 and hence this policy is set to OLD
# for now.
if(POLICY CMP0122)
  cmake_policy(SET CMP0122 OLD)
endif()

# Define project after settings. Inherit version from the parent project.
project(coinwrap VERSION ${PROJECT_VERSION})

# Build the Clp interface. The language wrappers are defined for this interface.
add_subdirectory(cpp)

# Define the native library.
add_library(coinwrap_cpp SHARED "")
set_target_properties(coinwrap_cpp PROPERTIES
  PREFIX ""
)

include(swig)
include(UseSWIG)

# Generate C++ and C# wrapper code and import the generated C++ wrapper with the
# Clp libraries into a new coinwrap_cpp.dll. At the end of this step, this DLL
# should be present in the build tree. We enable whole program optimization with
# at link time since the C++ code is compiled with it.
add_subdirectory(csharp)
target_link_libraries(coinwrap_cpp coinwrap_csharp)
target_link_options(coinwrap_cpp PUBLIC ${COMMON_LINK_OPTIONS})

# Build the dotnet nuget packages.
include(dotnet)
