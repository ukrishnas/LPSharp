function(get_patch_from_git VERSION_PATCH)
  find_package(Git QUIET)
  if(NOT GIT_FOUND)
    message(STATUS "Setting patch to default. Did not find git package.")
    set(PATCH 9999)
  else()
    # Count the number of commits in this branch.
    execute_process(COMMAND
      ${GIT_EXECUTABLE} rev-list HEAD --count
      RESULT_VARIABLE _OUTPUT_VAR
      OUTPUT_VARIABLE PATCH
      ERROR_QUIET
      WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR})
  endif()

  if (_OUTPUT_VAR)
    message(STATUS "Setting patch to default. Current source directory may not be a git repository.")
    set(PATCH 9999)
  else()
    string(REGEX REPLACE " |\n" "" PATCH ${PATCH})
  endif()
  set(${VERSION_PATCH} ${PATCH} PARENT_SCOPE)
endfunction()

function(set_version VERSION)
  # Get Major and Minor from Version.txt.
  file(STRINGS "Version.txt" VERSION_STR)
  foreach(STR ${VERSION_STR})
    if(${STR} MATCHES "MAJOR=(.*)")
      set(MAJOR ${CMAKE_MATCH_1})
    endif()
    if(${STR} MATCHES "MINOR=(.*)")
      set(MINOR ${CMAKE_MATCH_1})
    endif()
  endforeach()

  # Set to false if git patch is not required.
  if(TRUE)
    get_patch_from_git(PATCH)
  else()
    set(PATCH 9999)
  endif()

  # Force set tweak to differentiate from pubic versions.
  set(TWEAK 1)
  set(${VERSION} "${MAJOR}.${MINOR}.${PATCH}.${TWEAK}" PARENT_SCOPE)
endfunction()
