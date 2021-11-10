/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

/**
 * This interface provides a more user friendly interface to ClpSolver (the
 * source code for Clp standalone executable), ClpSimplex (the solver driver),
 * and ClpSolve (the solver options), and related classes. The interface reverse
 * engineers the settings used by Clp.exe, and you should get exactly the same
 * answer with this interface as you would with Clp.exe with 'dualS', 'primalS',
 * 'either', and 'barrier' commands with default options. In addition, it is
 * possible to change presolve method, pivot algorithm, starting basis, plus
 * minus 1-matrix, and other settings to customize the solver. The full
 * functionality of Clp is not exposed. The functions needed to solve our LP
 * problems have been exposed, but more functions can easily be added. This
 * interface is geared for SWIG wrapper generation.
 *
 * Clp model can be built using CoinBuild, CoinModel, CoinPackedMatrix and then
 * uploaded efficiently into ClpModel. CoinBuild is extremely lightweight and
 * best suited for adding entire column or row vectors. CoinModel has more
 * flexibility, like adding an element at a time, for the cost of being five
 * times slower than CoinBuild but four times faster than ClpModel. This
 * interface uses CoinModel. Add methods require row and column indices, and
 * CoinModelHash is used to convert names to indices.
 */

#ifndef COINWRAP_CLP_INTERFACE_H_
#define COINWRAP_CLP_INTERFACE_H_

#include <memory>
#include <vector>

#include "ClpSimplex.hpp"
#include "ClpSolve.hpp"
#include "CoinMessageHandler.hpp"
#include "CoinModel.hpp"
#include "CoinModelUseful.hpp"

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
 * For more information on initial basis methods, see:
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

    // Represents any starting basis that is not enumerated. This can be
    // returned by a get starting basis method but cannot be used to set the
    // starting basis.
    Other,
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
 * Represents the status of the problem.
 */
enum ClpStatus {
    // Unknown, for example before solve or if post solve says not optimal.
    Unknown = -1,

    // Optimal found.
    Optimal = 0,

    // Primal is infeasible.
    PrimalFeasible = 1,

    // Dual is infeasible.
    DualFeasible = 2,

    // Stopped due to maximum iteration or time limit reached.
    StoppedDueToLimits = 3,

    // Stopped due to errors.
    StoppedDueToErrors = 4,

    // Stopped by event handler.
    StoppedByEventHandler = 5,
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
    // automatically perturbs if it takes too long (1.0e-6 largest non-zero).
    // 101 means we are perturbed, and 102 means don't try perturbing again.
    // Default value is 50.
    int Perturbation() { return clp_->perturbation(); }
    void SetPerturbation(int value) { clp_->setPerturbation(value); }

    // Gets or sets the iteration limit. Note that this limit controls the
    // iterations performed by a solver stage, not the entire optimization. For
    // example, Clp will terminate the primal simplex upon reaching this limit,
    // the iteration count restarts from zero while solving using dual simplex.
    // To control the limit for the entire optimization, it is better to use
    // maximum seconds.
    int MaximumIterations() { return clp_->maximumIterations(); }
    void SetMaximumIterations(int value) { clp_->setMaximumIterations(value); }

    // Gets or sets the time limit from when set is called.
    double MaximumSeconds() { return clp_->maximumSeconds(); }
    void SetMaximumSeconds(double value) { clp_->setMaximumSeconds(value); }

    // Gets the solve time in milliseconds.
    double SolveTimeMs() { return solve_timems_; }

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
    bool ReadMps(const char *filename);

    // Writes the solver model to an MPS file.
    void WriteMps(const char *filename) { clp_->writeMps(filename); }

    // Sets the primal column pivot algorithm. Returns true on success, false if
    // the pivot algorithm is not supported or applicable.
    bool SetPrimalPivotAlgorithm(PivotAlgorithm pivot_algorithm);

    // Sets the dual row pivot algorithm. Returns true on success, false if the
    // pivot algorithm is not supported or applicable.
    bool SetDualPivotAlgorithm(PivotAlgorithm pivot_algorithm);

    // Gets or sets whether presolve should be enabled and the number of
    // presolve passes. Zero passes disabled presolve, any other number enables
    // it. Presolve analyzes the model to find such things as redundant
    // equations, equations which fix some variables, equations which can be
    // transformed into bounds, etc. Presolve is worth doing unless one knows
    // that it will have no effect. Presolve will return if nothing is being
    // taken out, so there is little need to fine tune the number of passes. Clp
    // presolve has a file option that is not available in this interface. The
    // default setting is on with Clp default number of passes.
    int PresolvePasses();
    void SetPresolvePasses(int passes);

    // Enables or disables making a plus minus 1-matrix. Clp will go slightly
    // faster if the matrix can be converted so that the elements are not stored
    // and are known to be unit. The main advantage is memory use. Clp may
    // automatically see if it can convert the problem so you should not need to
    // set this. Default is not make the plus minus 1-matrix.
    void MakePlusMinusOneMatrix(bool enable);

    // Gets or sets the starting basis method for dual simplex.
    StartingBasis DualStartingBasis();
    bool SetDualStartingBasis(StartingBasis basis);

    // Sets the starting basis method for primal simplex.
    StartingBasis PrimalStartingBasis();
    bool SetPrimalStartingBasis(StartingBasis basis);

    // Gets or sets the solve type.
    SolveType GetSolveType();
    void SetSolveType(SolveType solve_type);

    // Solves the optimization problem using the previously set solve type and
    // options. Use this method if you if you have customized the options. The
    // time and iterations may be affected by settings such as presolve, crash,
    // and dual and primal tolerances.
    void Solve();

    // Solves the optimization problem using dual simplex and associated
    // options. This method replicates the settings used by standalone
    // Clp.exe -dualS.
    void SolveUsingDualSimplex();

    // Solves the optimization problem using dual simplex with crash
    // starting basis.
    void SolveUsingDualCrash();

    // Solves the optimization problem using primal simplex and associated
    // options. This method replicates the settings used by standalone
    // Clp.exe -primalS.
    void SolveUsingPrimalSimplex();

    // Solves the optimization problem using primal simplex and idiot crash
    // starting basis.
    void SolveUsingPrimalIdiot();

    // Solves the optimization problem using either primal or dual simplex based
    // on dubious analysis of the model. The analysis looks at number of rows,
    // columns, elements in the model, settings like whether plus minus 1-matrix
    // is used, starting basis settings, and decides whether to call primal
    // (with or without sprint) or dual, and which starting basis method to use.
    // Use this method, if you do not know what is best for your model. This
    // method replicates the settings used by standalone Clp.exe -either.
    void SolveUsingEitherSimplex();

    // Solves the optimization problem using a barrier or interior point method.
    // It implements Mehrotra's primal dual predictor corrector algorithm. For
    // barrier code to be effective it needs a good Cholesky ordering and
    // factorization. The default method is native. Other methods like dense
    // (implemented by ClpCholeskyDense.cpp), and third party methods are not
    // available. See also
    // https://en.wikipedia.org/wiki/Mehrotra_predictor%E2%80%93corrector_method
    // This method replicates the settings used by standalone Clp.exe -barrier.
    void SolveUsingBarrierMethod();

    // Gets the value of the objective.
    double ObjectiveValue() { return clp_->objectiveValue(); }

    // Gets the number of iterations performed.
    int Iterations() { return clp_->numberIterations(); }

    // Gets the status of the problem.
    ClpStatus Status() { return static_cast<ClpStatus>(clp_->status()); }

    // Gets the secondary status. The values are described in ClpModel.hpp and
    // and updated based on comments in the code:
    // 0 - none.
    // 1 - primal infeasible because dual limit reached or probably primal
    //     infeasible but can't prove it (status was 4).
    // 2 - scaled problem optimal - unscaled problem has primal infeasibilities.
    // 3 - scaled problem optimal - unscaled problem has dual infeasibilities.
    // 4 - scaled problem optimal - unscaled problem has primal and dual infeasibilities.
    // 5 - giving up in primal with flagged variables.
    // 6 - failed due to empty problem check.
    // 7 - either preSolve or postSolve says not optimal.
    // 8 - failed due to bad element check.
    // 9 - status was 3 and stopped on time.
	// 10 - status was 3 but stopped as primal feasible.
    int SecondaryStatus() { return clp_->secondaryStatus(); }

    // Gets the primal column solution. This is the solution of the variables to
    // achieve the objective in the primal simplex. The dot product of this
    // vector and the objective is the objective result.
    void PrimalColumnSolution(std::vector<double> &vec) {
        int size = clp_->numberColumns();
        const double *start = clp_->getColSolution();
        vec.assign(start, start + size);
    }

    // Gets the dual column solution, also known as the reduced cost vector d =
    // c - A^T y in the dual simplex. It is the byproduct of dual simplex and
    // represents how much the cost coefficient needs to be decrease before the
    // corresponding variable could be considered in the solution.
    void DualColumnSolution(std::vector<double> &vec) {
        int size = clp_->numberColumns();
        const double *start = clp_->getReducedCost();
        vec.assign(start, start + size);
    }

    // Gets the primal row solution, also called the row activity by the code.
    void PrimalRowSolution(std::vector<double> &vec) {
        int size = clp_->numberRows();
        const double *start = clp_->getRowActivity();
        vec.assign(start, start + size);
    }

    // Gets the dual row solution, also called the row price by the code.
    void DualRowSolution(std::vector<double> &vec) {
        int size = clp_->numberRows();
        const double *start = clp_->getRowPrice();
        vec.assign(start, start + size);
    }

    // Gets the objective from the model. The objective is the cost vector. The
    // dot product of this vector and the primal column solution is the
    // objective value.
    void Objective(std::vector<double> &vec) {
        int size = clp_->numberColumns();
        double *start = clp_->objective();
        vec.assign(start, start + size);
    }

    // Starts a new model and clears the old model build object. This should be
    // the first call when building a model. This does not affect the state of
    // the solver.
    void StartModel();

    // Creates a variable with specified name, and upper and lower bounds.
    // It returns the column index assigned to the variable, or -1 if the name
    // is already in use.
    int AddVariable(const char *column_name, double lower_bound, double upper_bound);

    // Creates a constraint with specified name, and upper and lower bounds. It
    // returns the row index assigned to the constraint or -1 if the name is
    // already in use.
    int AddConstraint(const char *row_name, double lower_bound, double upper_bound);

    // Sets a coefficient for an element in the constraint matrix using indices.
    void SetCoefficient(int row_index, int column_index, double value);
    
    // Sets a coefficient for an element in the constraint matrix using names.
    bool SetCoefficient(const char *row_name, const char *column_name, double value);
    
    // Sets the coefficient for a column in the objective using column index.
    void SetObjective(int column_index, double value);

    // Sets the coefficient for a column in the objective using column name.
    bool SetObjective(const char *column_name, double value);

    // Loads the model build object into the solver. This should be the final
    // call after constructing the model.
    void LoadModel();

 private:
    // The Clp solver.
    std::unique_ptr<ClpSimplex> clp_;

    // The Clp options.
    std::unique_ptr<ClpSolve> solve_options_;

    // The message handler.
    std::unique_ptr<CoinMessageHandler> message_handler_;

    // The primal column pivot algorithm.
    PivotAlgorithm primal_pivot_algorithm_;

    // The dual row pivot algorithm.
    PivotAlgorithm dual_pivot_algorithm_;

    // The psi weight for positive edge pivot selection methods.
    double positive_edge_psi_;

    // The solve time.
    double solve_timems_;

    // The model build object.
    CoinModel coin_model_;

    // The hash table mapping row and column names to indices.
    CoinModelHash row_hash_;
    CoinModelHash column_hash_;
};

} // namespace coinwrap

#endif // COINWRAP_CLP_INTERFACE_H_
