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

#include <memory>
#include <string>

#include "ClpSimplex.hpp"
#include "ClpSolve.hpp"
#include "CoinMessageHandler.hpp"

namespace coinwrap {

/**
 * Represents the pivot algorithm. This is a subset of Clp's pivot algorithms.
 * We have retained Devex based pivot methods and excluded Dantzig based methods
 * since the latter do not perform well. Devex is an efficient approximation of
 * steepest edge pricing pivot rule. It uses a reference framework of a subset
 * of variables, and a dynamic weighting over this subset to find the
 * approximate steepest edge. For more information, see:
 *
 * - Paula M.J. Harris. Pivot Selection Methods of the Devex LP code.
 * - J.J. Forrest and D. Goldfarb. Steepest-edge simplex algorithms for linear
 *   programming.
 * - M. Towhidi, J. Desrosiers, F. Soumis. The positive edge criterion within
 *   COIN-OR's CLP.
 */
enum PivotAlgorithm {
    // For primal, it switches between exact Devex and partial Devex based on
    // factorization. For dual, it switches between partial Devex and steepest
    // based on factorization. This is the Clp default and performs well for
    // most models.
    Automatic = 0,

    // Implements Devex pivot selection method. This is only available for
    // primal pivot column selection and not for dual pivot row selection.
    ExactDevex,

    // Weights are updated as normal but only part of the nonbasic variables are
    // scanned in each iteration.
    PartialDevex,

    // Full steepest increases the number of variables in the reference
    // framework of Devex pivot rules.
    Steepest,

    // Combines Devex with positive edge criterion that selects incoming
    // variables to try to avoid degenerate moves. Performance speed up is
    // achieved when degeneracy level exceeds 25%.
    PositiveEdgeSteepest,
};

/*
 * Represents the technique to select the starting basis. Only a subset of Clp
 * choices are available, and extra options for the basis method cannot be set.
 * For more information, see:
 *
 * R. Bixby. Implementing the simplex method: the initial basis. Part II.
 */
enum StartingBasis {
    // Use the Clp default which is all slack variables for dual. For primal, it
    // is a combination crash and sprint based on the model.
    Default = 0,

    // All slack variables form the basis. The inclusion of all slack variables
    // is motivated by the fact that a significant portion of them will be in
    // the optimal basis. On average, the dual method works best will this approach.
    AllSlack,

    // Crash procedure, originally developed in CPLEX, and described in Bixby's
    // paper. It uses a heuristic factorization to eliminate artificial
    // variables. This method is effective in dealing with certain problem
    // structures. See ClpSimplex::crash().
    Crash,

    // Idiot crash is a simple method that is good at getting an approximate
    // solution for problems that have some homogeneous structure.
    Idiot,

    // This approach engages a driver in ClpSolve::initialSolve() that
    // constructs the basis differently. It works best if you can get to
    // feasible easily. Sprint is only available with primal method and has no
    // effect in other methods.
    Sprint,
};

/**
 * Represents the solve type. This is a subset of the solve types offered by Clp.
 */
enum SolveType {
    // Dual simplex.
    Dual = 0,

    // Primal simplex.
    Primal,

    // Either primal or dual simplex.
    Either,

    // Barrier method.
    Barrier,
};

/**
 *  Represents the wrapper for the Clp solver and its parameters.
 */
class ClpInterface {
 public:
    // Initializes a new instance of the class.
    explicit ClpInterface();

    // Destroys an instance of the class.
    ~ClpInterface();

    // Gets or sets the number of presolve passes.
    int PresolvePasses() { return presolve_passes_; }
    void SetPresolvePasses(int value) { presolve_passes_ = value; }

    // Gets or sets the optimization direction. Use +1 for minimize, -1 for
    // maximize, and 0 for ignore. Default value is minimize.
    double OptimizationDirection() { return clp_->optimizationDirection(); }
    void SetOptimizationDirection(double value) { clp_->setOptimizationDirection(value); }

    // Gets or sets the dual tolerance. For an optimal solution no dual
    // infeasibility may exceed this value. Normally the default tolerance is
    // fine, but one may want to increase it a bit if the dual simplex algorithm
    // seems to be having a hard time. One method which can be faster is to use
    // a large tolerance e.g. 1.0e-4 and the dual simplex algorithm and then to
    // clean up the problem using the primal simplex algorithm with the correct
    // tolerance (remembering to switch off presolve for this final short clean
    // up phase). Range of values 1e-20 to 1.79769e+308. Default value is 1e-07.
    double DualTolerance() { return clp_->currentDualTolerance(); }
    void SetDualTolerance(double value) { clp_->setDualTolerance(value); }

    // Gets or sets the primal tolerance. For a feasible solution no primal
    // infeasibility, i.e., constraint violation, may exceed this value.
    // Normally the default tolerance is fine, but one may want to increase
    // it a bit if the primal simplex algorithm seems to be having a hard time.
    // Range of values is 1e-20 to 1.79769e+308, default is 1e-07.
    double PrimalTolerance() { return clp_->currentPrimalTolerance(); }
    void SetPrimalTolerance(double value) { clp_->setPrimalTolerance(value); }

    // Gets or sets the primal weight. Initially algorithm acts as if it costs
    // this much to be infeasible. The primal algorithm in Clp is a single phase
    // algorithm as opposed to a two phase algorithm where you first get
    // feasible then optimal. So Clp is minimizing this weight times the sum of
    // primal infeasibilities plus the true objective function (in minimization
    // sense).  Too high a value may mean more iterations, while too low a value
    // means the algorithm may iterate into the wrong directory for long and
    // then has to increase the weight in order to get feasible. Range of values
    // is 1e-20 to 1.79769e+308. Default value is 1e+10.
    double PrimalWeight() { return clp_->infeasibilityCost(); }
    void SetPrimalWeight(double value) { clp_->setInfeasibilityCost(value); }

    // Gets or sets the pricing factor for positive edge criterion pivot rule.
    // which selects incoming variables to avoid degenerate moves. Variables not
    // in the promising set have their infeasibility weight multiplied by psi,
    // so 0.01 would mean that if there were any promising variables, then they
    // would always be chosen, while 1.0 effectively switches the algorithm off.
    // This value only takes effect if the pivoting algorithm is positive edge
    // criterion. Range of values is 0 to 1.1. Default value 0.5.
    double PositiveEdgePsi() { return positive_edge_psi_; }
    void SetPositiveEdgePsi(double value) {
        if (value >= 0 && value <= 1.1) positive_edge_psi_ = value;
    }

    // Gets or sets the perturbation value. 50 switches on perturbation. 100
    // automatically perturns if it takes too long (1.0e-6 largest non-zero).
    // 101 means we are perturbed, and 102 means don't try perturbing again.
    // Default value is 100.
    int Perturbation() { return clp_->perturbation(); }
    void SetPerturbation(int value) { clp_->setPerturbation(value); }

    // Sets the log level. Values range [0, 4] with 0 being none and 4 being
    // verbose.
    void SetLogLevel(int level);

    // Resets the solution inside the solver but leaves the settings unchanged.
    // Without a reset, a subsequent call to solve will use previously computed
    // values as the basis and finish very quickly. To collect benchmark
    // results, please call this method before successive invocations of solve
    // methods.
    void Reset();

    // Loads a model from an MPS file into the solver. Returns true on success,
    // false otherwise.
    bool ReadMps(std::string filename);

    // Sets the primal column pivot algorithm. Returns true on success, false if
    // the pivot algorithm is not supported or applicable.
    bool SetPrimalPivotAlgorithm(PivotAlgorithm pivot_algorithm);

    // Sets the dual row pivot algorithm. Returns true on success, false if the
    // pivot algorithm is not supported or applicable.
    bool SetDualPivotAlgorithm(PivotAlgorithm pivot_algorithm);

    // Sets whether presolve should be enabled and the number of presolve
    // passes. Presolve analyzes the model to find such things as redundant
    // equations, equations which fix some variables, equations which can be
    // transformed into bounds, etc. Presolve is worth doing unless one knows
    // that it will have no effect. Presolve will return if nothing is being
    // taken out, so there is little need to fine tune the number of passes. Clp
    // presolve has a file option that is not available in this interface. The
    // default setting is on with Clp default number of passes.
    void SetPresolve(bool enable, int number);

    // Enables or disables making a plus minus 1-matrix. Clp will go slightly
    // faster if the matrix can be converted so that the elements are not stored
    // and are known to be unit. The main advantage is memory use. Clp may
    // automatically see if it can convert the problem so you should not need to
    // set this. Default is not make the plus minus 1-matrix.
    void MakePlusMinusOneMatrix(bool enable);

    // Sets the starting basis method for dual simplex.
    bool SetDualStartingBasis(StartingBasis basis);

    // Sets the starting basis method for primal simplex.
    bool SetPrimalStartingBasis(StartingBasis basis);

    // Sets the solve type.
    void SetSolveType(SolveType solve_type);

    // Solves the optimization problem using the previously set solve type and
    // options. Use this method if you if you have customized the options. The
    // time and iterations may be affected by settings such as presolve, crash,
    // and dual and primal tolerances.
    void Solve();

    // Solves the optimization problem using dual simplex and associated
    // options. This method approximates the settings used by standalone
    // Clp.exe -dualS.
    void SolveUsingDualSimplex();

    // Solves the optimization problem using primal simplex and associated
    // options. This method replicates the settings used by standalone
    // Clp.exe -primalS.
    void SolveUsingPrimalSimplex();

    // Solves the optimization problem using either primal or dual simplex based
    // on dubious analysis of the model. The analysis looks at number of rows,
    // columns, elements in the model, settings like whether plus minus 1-matrix
    // is used, starting basis settings, and decides whether to call primal
    // (with or without sprint) or dual, and which starting basis method to use.
    // Use this method, if you do not know what is best for your model.
    void SolveUsingEitherSimplex();

    // Solves the optimization problem using a barrier or interior point method.
    // It implements Mehrotra's primal dual predictor corrector algorithm. For
    // barrier code to be effective it needs a good Cholesky ordering and
    // factorization. The default method is native, dense (implemented by
    // ClpCholeskyDense.cpp), and choices are native ordering and factorization
    // is not state of the art, although acceptable. Possible options for
    // cholesky are: native, dense. See also
    // https://en.wikipedia.org/wiki/Mehrotra_predictor%E2%80%93corrector_method
    void SolveUsingBarrierMethod();

 private:
    // The Clp solver.
    std::unique_ptr<ClpSimplex> clp_;

    // The Clp options.
    std::unique_ptr<ClpSolve> solve_options_;

    // The message handler.
    std::unique_ptr<CoinMessageHandler> message_handler_;

    // The number of presolve passes. The default value is 10.
    int presolve_passes_;

    // The primal column pivot algorithm.
    PivotAlgorithm primal_pivot_algorithm_;

    // The dual row pivot algorithm.
    PivotAlgorithm dual_pivot_algorithm_;

    // The psi weight for positive edge pivot selection methods.
    double positive_edge_psi_;
};

} // namespace coinwrap

#endif // COINWRAP_CLP_INTERFACE_H_
