function(get_commit_from_git COMMIT_COUNT CLONE_DIR)
  find_package(Git QUIET)
  if(NOT GIT_FOUND)
    message(STATUS "Setting commit count to default. Did not find git package.")
    set(COMMIT_COUNT 9999)
  else()
    # Count the number of commits in this branch.
    execute_process(COMMAND
      ${GIT_EXECUTABLE} rev-list HEAD --count
      RESULT_VARIABLE _OUTPUT_VAR
      OUTPUT_VARIABLE _COMMIT_COUNT
      ERROR_QUIET
      WORKING_DIRECTORY ${CLONE_DIR})
  endif()

  if (_OUTPUT_VAR)
    message(STATUS "Setting commit count to default. Current source directory may not be a git repository.")
    set(COMMIT_COUNT 9999)
  else()
    string(REGEX REPLACE " |\n" "" _COMMIT_COUNT ${_COMMIT_COUNT})
  endif()
  set(${COMMIT_COUNT} ${_COMMIT_COUNT} PARENT_SCOPE)
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

  # Set to false if git commit count is not required.
  if(TRUE)
    get_commit_from_git(PATCH ${CMAKE_CURRENT_SOURCE_DIR}/../Clp)
    get_commit_from_git(TWEAK ${CMAKE_CURRENT_SOURCE_DIR})
  else()
    set(PATCH 1)
    set(TWEAK 1)
  endif()

  set(${VERSION} "${MAJOR}.${MINOR}.${PATCH}.${TWEAK}" PARENT_SCOPE)
endfunction()