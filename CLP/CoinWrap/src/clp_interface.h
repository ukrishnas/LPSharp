/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

/**
 * This project (CoinWrap) wraps ClpSimplex (the solver class) and ClpSolve
 * (class for options) methods and referenced types. SWIG is used to provide the
 * next level of wrappers in different languages, e.g. C# or Python. This
 * approach is inspired by the work of Google OR-Tools development team. See
 * https://github.com/google/or-tools/blob/stable/ortools/linear_solver/clp_interface.cc.
 *
 * Wrapped classes and methods are listed below.
 *
 * - ClpSolve
 *   - setPresolveType
 *   - setSolveType
 *   - setSpecialOption
 * - ClpSimplex
 *   - addColumn
 *   - getColumnStatus
 *   - getIterationCount
 *   - getRowPrice
 *   - getRowStatus
 *   - modifyCoefficient
 *   - passInMessageHandler (passes CoinMessageHandler)
 *   - resize
 *   - setColumnBounds
 *   - setColumnName
 *   - setDualTolerance
 *   - setLogLevel
 *   - setObjectiveCoefficient
 *   - setObjectiveOffset
 *   - setOptimizationDirection
 *   - setPrimalTolerance
 *   - setRowBounds
 *   - setRowName
 *   - setStrParam
 */

#ifndef COINWRAP_CLP_INTERFACE_H_
#define COINWRAP_CLP_INTERFACE_H_

#include "ClpSimplex.hpp"
#include "ClpSolve.hpp"

namespace coinwrap {

/// Represents the wrapper for the Clp solver and its parameters.
///
class ClpInterface  {
 public:
    // Initializes a new instance of the class.
     explicit ClpInterface();

     // Destroys an instance of the class.
     ~ClpInterface();

    // Loads a model from an MPS file into the solver. Returns true on success,
    // false otherwise.
    bool ReadMps(std::string filename);

    // Solves the optimization problem.
    void Solve();

 private:
     // The Clp solver.
     std::unique_ptr<ClpSimplex> clp_;

     // The Clp options.
     std::unique_ptr<ClpSolve> solveOptions_;
};

} // namespace coinwrap

#endif // COINWRAP_CLP_INTERFACE_H_