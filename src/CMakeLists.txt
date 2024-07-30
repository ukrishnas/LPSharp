# Enumerate all the files since some files in the folder are not used in the
# library. CMake recommends enumerating files instead of using glob patterns so
# that dependencies are not missed, so the extra work is worth it.
target_sources(${LIB_CLP} PRIVATE
  ClpCholeskyBase.cpp
  ClpCholeskyDense.cpp
  ClpConstraint.cpp
  ClpConstraintLinear.cpp
  ClpConstraintQuadratic.cpp
  ClpDualRowDantzig.cpp
  ClpDualRowPivot.cpp
  ClpDualRowSteepest.cpp
  ClpDummyMatrix.cpp
  ClpDynamicExampleMatrix.cpp
  ClpDynamicMatrix.cpp
  ClpEventHandler.cpp
  ClpFactorization.cpp
  ClpGubDynamicMatrix.cpp
  ClpGubMatrix.cpp
  ClpHelperFunctions.cpp
  ClpInterior.cpp
  ClpLinearObjective.cpp
  ClpLsqr.cpp
  ClpMatrixBase.cpp
  ClpMessage.cpp
  ClpModel.cpp
  ClpNetworkBasis.cpp
  ClpNetworkMatrix.cpp
  ClpNode.cpp
  ClpNonLinearCost.cpp
  ClpObjective.cpp
  ClpPackedMatrix.cpp
  ClpPdco.cpp
  ClpPdcoBase.cpp
  ClpPlusMinusOneMatrix.cpp
  ClpPredictorCorrector.cpp
  ClpPresolve.cpp
  ClpPrimalColumnDantzig.cpp
  ClpPrimalColumnPivot.cpp
  ClpPrimalColumnSteepest.cpp
  ClpQuadraticObjective.cpp
  ClpSimplex.cpp
  ClpSimplexDual.cpp
  ClpSimplexNonlinear.cpp
  ClpSimplexOther.cpp
  ClpSimplexPrimal.cpp
  ClpSolve.cpp
  Idiot.cpp
  IdiSolve.cpp
  ClpPESimplex.cpp
  ClpPEPrimalColumnDantzig.cpp
  ClpPEPrimalColumnSteepest.cpp
  ClpPEDualRowDantzig.cpp
  ClpPEDualRowSteepest.cpp
)

target_sources(${LIB_CLP} PUBLIC
  ClpCholeskyBase.hpp
  ClpCholeskyDense.hpp
  ClpCholeskyMumps.hpp
  ClpCholeskyPardiso.hpp
  ClpCholeskyTaucs.hpp
  ClpCholeskyUfl.hpp
  ClpCholeskyWssmp.hpp
  ClpConstraint.hpp
  ClpConstraintLinear.hpp
  ClpConstraintQuadratic.hpp
  ClpDualRowDantzig.hpp
  ClpDualRowPivot.hpp
  ClpDualRowSteepest.hpp
  ClpDummyMatrix.hpp
  ClpDynamicExampleMatrix.hpp
  ClpDynamicMatrix.hpp
  ClpEventHandler.hpp
  ClpFactorization.hpp
  ClpGubDynamicMatrix.hpp
  ClpGubMatrix.hpp
  ClpHelperFunctions.hpp
  ClpInterior.hpp
  ClpLinearObjective.hpp
  ClpLsqr.hpp
  ClpMatrixBase.hpp
  ClpMessage.hpp
  ClpModel.hpp
  ClpNetworkBasis.hpp
  ClpNetworkMatrix.hpp
  ClpNode.hpp
  ClpNonLinearCost.hpp
  ClpObjective.hpp
  ClpPackedMatrix.hpp
  ClpParameters.hpp
  ClpPdco.hpp
  ClpPdcoBase.hpp
  ClpPlusMinusOneMatrix.hpp
  ClpPredictorCorrector.hpp
  ClpPresolve.hpp
  ClpPrimalColumnDantzig.hpp
  ClpPrimalColumnPivot.hpp
  ClpPrimalColumnSteepest.hpp
  ClpQuadraticObjective.hpp
  ClpSimplex.hpp
  ClpSimplexDual.hpp
  ClpSimplexNonlinear.hpp
  ClpSimplexOther.hpp
  ClpSimplexPrimal.hpp
  ClpSolve.hpp
  Idiot.hpp
  ClpPESimplex.hpp
  ClpPEPrimalColumnDantzig.hpp
  ClpPEPrimalColumnSteepest.hpp
  ClpPEDualRowDantzig.hpp
  ClpPEDualRowSteepest.hpp
)

target_sources(${LIB_CLP} PUBLIC
  ClpConfig.h
  configall_system.h
  configall_system_msc.h
  config_clp_default.h
  config_default.h
)

set(CLP_COMPILE_DEFINITIONS ${COMMON_COMPILE_DEFINITIONS})
list(APPEND CLP_COMPILE_DEFINITIONS
  "CLPLIB_BUILD"
  "_LIB"
)
set(CLP_COMPILE_OPTIONS ${COMMON_COMPILE_OPTIONS})

target_compile_definitions(${LIB_CLP} PRIVATE ${CLP_COMPILE_DEFINITIONS})
target_compile_options(${LIB_CLP} PRIVATE ${COMMON_COMPILE_OPTIONS})
target_link_options(${LIB_CLP} PRIVATE ${COMMON_LINK_OPTIONS})
target_include_directories(${LIB_CLP} PUBLIC
  ${CMAKE_CURRENT_SOURCE_DIR}
)
target_link_libraries(${LIB_CLP} ${LIB_COINUTILS})

# Clp standalone executable.
set(EXE_CLP clp)
add_executable(${EXE_CLP})
target_sources(${EXE_CLP} PRIVATE
  CbcOrClpParam.cpp
  ClpMain.cpp
  ClpSolver.cpp
  MyEventHandler.cpp
  MyMessageHandler.cpp
  unitTest.cpp
  CbcOrClpParam.hpp
  MyEventHandler.hpp
  MyMessageHandler.hpp
)
set(EXECLP_COMPILE_DEFINITIONS ${COMMON_COMPILE_DEFINITIONS})
list(APPEND EXECLP_COMPILE_DEFINITIONS
  "COIN_HAS_CLP"
  "CLPLIB_BUILD"
  "COIN_MSVS"
  "_CONSOLE"
)
target_compile_definitions(${EXE_CLP} PRIVATE ${EXECLP_COMPILE_DEFINITIONS})
target_compile_options(${EXE_CLP} PRIVATE ${COMMON_COMPILE_OPTIONS})
target_link_options(${EXE_CLP} PRIVATE ${COMMON_LINK_OPTIONS})
target_link_libraries(${EXE_CLP} 
  ${LIB_COINUTILS}
  ${LIB_CLP}
)
