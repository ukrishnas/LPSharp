cmake_minimum_required(VERSION 3.15)

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

project(coinwrap)

# Create the Clp interface library.
add_subdirectory(cpp)

# Create the Clp interface csharp library.
set(BUILD_SWIG ON)
# add_subdirectory(csharp)
