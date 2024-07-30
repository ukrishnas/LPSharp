/*
 * Copyright (c) 2024 Umesh Krishnaswamy
 * This code is licensed under the terms of the MIT License.
 */

#include "clp_interface.h"

#include <cstdio>
#include <cstdlib>
#include <iostream>
#include <vector>

#include "ClpDualRowSteepest.hpp"
#include "ClpPrimalColumnSteepest.hpp"
#include "ClpPEDualRowSteepest.hpp"
#include "ClpPEPrimalColumnSteepest.hpp"
#include "ClpSimplex.hpp"
#include "ClpSolve.hpp"
#include "CoinBuild.hpp"
#include "CoinPackedMatrix.hpp"
#include "CoinTime.hpp"

namespace coinwrap {

// The default number of presolve passes.
#define DefaultPresolvePasses 10

// The default perturbation value.
#define DefaultPerturbation 50

// The default number of idiot crash passes to perform when this is the starting
// basis method.
#define DefaultIdiotPasses 1

// Initializes a new instance of the ClpInterface with default values. The
// values used here are the defaults.
ClpInterface::ClpInterface() :
    clp_(new ClpSimplex),
    solve_options_(new ClpSolve),
    message_handler_(new CoinMessageHandler),
    dual_pivot_algorithm_(PivotAlgorithm::Automatic),
    primal_pivot_algorithm_(PivotAlgorithm::Automatic),
    positive_edge_psi_(0.5),
    row_build_object_(),
    column_build_object_(1) {
    
    // Initialize the message handler to no logging.
    message_handler_->setLogLevel(0);
    message_handler_->setPrefix(false);
    clp_->passInMessageHandler(message_handler_.get());
    clp_->setLogLevel(0);
}

ClpInterface::~ClpInterface() {}

void ClpInterface::SetLogLevel(int level) {
    // Set the log level in the message handler. There is another log level by
    // facility in the messange handler that is set using
    // CoinMessageHandler::setLogLevel (which, level). It slows down
    // performance, and hence is not used.
    clp_->setLogLevel(level);
}

void ClpInterface::Reset() {
    // Sets up all slack basis and resets solution to as it was after initial
    // load or readMps.
    clp_->allSlackBasis(true);
}

bool ClpInterface::ReadMps(const char *filename) {
    FILE *fp;
    fp = fopen(filename, "r");
    if (fp) {
        fclose(fp);
    } else {
        return false;
    }

    // Always keep names and do not ignore errors.
    int status = clp_->readMps(filename, true, false);
    return status == 0;
}

void ClpInterface::WriteMps(const char *filename, int number_format) {
    // The second parameter controls accuracy. Zero is default, 1 is extended
    // accuracy, and 2 is IEEE hexadecimal. MPS format is fixed by default, but
    // the format changes to free if using extended accuracy or the model has a
    // row or column name that is greater than 8 characters.
    if (number_format > 2 || number_format < 0) {
        number_format = 0;
    }
    clp_->writeMps(filename, number_format);
}

bool ClpInterface::SetDualPivotAlgorithm(PivotAlgorithm pivot_algorithm) {
    if (dual_pivot_algorithm_ == pivot_algorithm) {
        // Nothing to do.
        return false;
    }

    // Clp has a complex numbering system for pivot rules. This is taken from
    // Clp standalone code ClpSolver.cpp.
    if (pivot_algorithm == PivotAlgorithm::Automatic) {
        ClpDualRowSteepest steep(3);
        clp_->setDualRowPivotAlgorithm(steep);
    } else if (pivot_algorithm == PivotAlgorithm::PartialDevex) {
        ClpDualRowSteepest steep(2);
        clp_->setDualRowPivotAlgorithm(steep);
    } else if (pivot_algorithm == PivotAlgorithm::Steepest) {
        ClpDualRowSteepest steep(1);
        clp_->setDualRowPivotAlgorithm(steep);       
    } else if (pivot_algorithm == PivotAlgorithm::PositiveEdgeSteepest) {
        ClpPEDualRowSteepest steep(positive_edge_psi_);
        clp_->setDualRowPivotAlgorithm(steep);
    } else {
        // Unknown pivot algorithm.
        return false;
    }

    dual_pivot_algorithm_ = pivot_algorithm;
    return true;
}

bool ClpInterface::SetPrimalPivotAlgorithm(PivotAlgorithm pivot_algorithm) {
    if (primal_pivot_algorithm_ == pivot_algorithm) {
        // Nothing to do.
        return false;
    }

    // Clp has a complex numbering system for pivot rules. This is taken from
    // Clp standalone code ClpSolver.cpp.
    if (pivot_algorithm == PivotAlgorithm::Automatic) {
        ClpPrimalColumnSteepest steep(3);
        clp_->setPrimalColumnPivotAlgorithm(steep);
    } else if (pivot_algorithm == PivotAlgorithm::ExactDevex) {
        ClpPrimalColumnSteepest steep(0);
        clp_->setPrimalColumnPivotAlgorithm(steep);    
    } else if (pivot_algorithm == PivotAlgorithm::PartialDevex) {
        ClpPrimalColumnSteepest steep(4);
        clp_->setPrimalColumnPivotAlgorithm(steep);       
    } else if (pivot_algorithm == PivotAlgorithm::Steepest) {
        ClpPrimalColumnSteepest steep(1);
        clp_->setPrimalColumnPivotAlgorithm(steep);   
    } else if (pivot_algorithm == PivotAlgorithm::PositiveEdgeSteepest) {
        ClpPEPrimalColumnSteepest steep(positive_edge_psi_);
        clp_->setPrimalColumnPivotAlgorithm(steep);
    } else {
        // Unknown pivot algorithm.
        return false;
    }

    primal_pivot_algorithm_ = pivot_algorithm;
    return true;
}

int ClpInterface::PresolvePasses() {
    ClpSolve::PresolveType presolve_type = solve_options_->getPresolveType();
    if (presolve_type == ClpSolve::presolveOff) {
        return 0;
    }

    int presolve_passes = solve_options_->getPresolvePasses();
    return presolve_passes;
}

void ClpInterface::SetPresolvePasses(int passes) {
    if (passes == 0) {
        solve_options_->setPresolveType(ClpSolve::presolveOff, 0);
    } else {
        solve_options_->setPresolveType(ClpSolve::presolveOn, passes);

        // This setting is important. It fixes infeasibility by allowing
        // presolve transforms to arbitrarily ignore infeasibility and set
        // arbitrary feasible bounds. This coupled with perturbation setting
        // causes the model to be perturbed in primal and dual to fix
        // infeasibility.
        solve_options_->setPresolveActions(32768);
    }
}

void ClpInterface::MakePlusMinusOneMatrix(bool enable) {
    // The special option arguments are which=3 is for controlling whether to
    // make plus minus 1-matrix. Value=0 means make the matrix, and 1 means do
    // not make the matrix.
    if (enable) {
        solve_options_->setSpecialOption(3, 0);
    } else {
        solve_options_->setSpecialOption(3, 1);
    }
}

StartingBasis ClpInterface::DualStartingBasis() {
    int option = solve_options_->getSpecialOption(0);
    if (option == 0) {
        return StartingBasis::Default;
    } else if (option == 1) {
        return StartingBasis::Crash;
    } else if (option == 2) {
        return StartingBasis::Idiot;
    }
    
    return StartingBasis::Other;
}

bool ClpInterface::SetDualStartingBasis(StartingBasis basis) {
    // The special option arguments are which and value. Which is 0 for setting
    // dual starting basis. The values are from comments in ClpSolve.hpp and
    // code in ClpSolve.cpp. On average, dual simplex seems to perform better
    // with no basis.
    if (basis == StartingBasis::Default || basis == StartingBasis::AllSlack) {
        solve_options_->setSpecialOption(0, 0);
    } else if (basis == StartingBasis::Crash) {
        solve_options_->setSpecialOption(0, 1);
    } else if (basis == StartingBasis::Idiot) {
        solve_options_->setSpecialOption(0, 2, DefaultIdiotPasses);
    } else {
        // Unsupported basis method.
        return false;
    }
    
    return true;
}

StartingBasis ClpInterface::PrimalStartingBasis() {
    int option = solve_options_->getSpecialOption(1);
    if (option == 0) {
        return StartingBasis::Default;
    } else if (option == 1) {
        return StartingBasis::Crash;
    } else if (option == 2) {
        return StartingBasis::Idiot;
    } else if (option == 3) {
        return StartingBasis::Sprint;
    } else if (option == 4) {
        return StartingBasis::AllSlack;
    }
    
    return StartingBasis::Other;
}

bool ClpInterface::SetPrimalStartingBasis(StartingBasis basis) {
    // The special option arguments are which and value. Which is 1 for setting
    // primal starting basis. The values are from comments in ClpSolve.hpp and
    // code in ClpSolve.cpp. This method does not offer all the options of Clp.
    // It does not set extra options for sprint or crash, or support the
    // additional combinations of using crash, idiot, and sprint.
    if (basis == StartingBasis::Default) {
        solve_options_->setSpecialOption(1, 0);
    } else if (basis == StartingBasis::AllSlack) {
        solve_options_->setSpecialOption(1, 4);
    } else if (basis == StartingBasis::Crash) {
        solve_options_->setSpecialOption(1, 1);
    } else if (basis == StartingBasis::Idiot) {
        solve_options_->setSpecialOption(1, 2, DefaultIdiotPasses);
    } else if (basis == StartingBasis::Sprint) {
        solve_options_->setSpecialOption(1, 3);
    } else {
        // Unsupported basis method.
        return false;
    }

    return true;
}

SolveType ClpInterface::GetSolveType() {
    ClpSolve::SolveType solve_type = solve_options_->getSolveType();

    if (solve_type == ClpSolve::usePrimal || solve_type == ClpSolve::usePrimalorSprint) {
        return SolveType::Primal;
    } else if (solve_type == ClpSolve::automatic) {
        return SolveType::Either;
    } else if (solve_type == ClpSolve::useBarrier) {
        return SolveType::Barrier;
    } else {
        // Default is always returned as dual.
        return SolveType::Dual;
    }
}

void ClpInterface::SetSolveType(SolveType solve_type) {
    switch(solve_type) {
        default:
        case SolveType::Dual:
            solve_options_->setSolveType(ClpSolve::useDual);
            break;

        case SolveType::Primal: {
            // Based on whether sprint starting basis is selected, choose the solve type
            // to be primal with sprint, or just primal. Primal with sprint activates
            // some code in the driver in ClpSimplex::initialSolve() to set up the
            // starting basis.
                int option = solve_options_->getSpecialOption(1);
                if (option == 0 || option == 3) {
                    solve_options_->setSolveType(ClpSolve::usePrimalorSprint);
                } else {
                    solve_options_->setSolveType(ClpSolve::usePrimal);
                }
            }
            break;

        case SolveType::Either:
            solve_options_->setSolveType(ClpSolve::automatic);
            break;

        case SolveType::Barrier:
            solve_options_->setSolveType(ClpSolve::useBarrier);
            break;
    }
}

void ClpInterface::Solve() {
#ifdef DEBUG
    std::cout << "Special options = " << clp_->specialOptions() << std::endl;
    std::cout << "More special options = " << clp_->moreSpecialOptions() << std::endl;
    for (int i = 0; i < 3; i++) {
        std::cout << "Independent option " << i << " = " << solve_options_->independentOption(i) << std::endl;
    }
#endif
    double time1 = CoinCpuTime();
    clp_->initialSolve(*solve_options_);
    solve_timems_ = (CoinCpuTime() - time1) * 1000;
}

void ClpInterface::SolveUsingDualSimplex() {
    SetDualPivotAlgorithm(PivotAlgorithm::Automatic);
    SetPresolvePasses(DefaultPresolvePasses);
    SetDualStartingBasis(StartingBasis::Default);
    SetPerturbation(DefaultPerturbation);
    MakePlusMinusOneMatrix(false);

    SetSolveType(SolveType::Dual);
    Solve();
}

void ClpInterface::SolveUsingDualCrash() {
    SetDualPivotAlgorithm(PivotAlgorithm::Automatic);
    SetPresolvePasses(DefaultPresolvePasses);
    SetDualStartingBasis(StartingBasis::Crash);
    SetPerturbation(DefaultPerturbation);
    MakePlusMinusOneMatrix(false);

    SetSolveType(SolveType::Dual);
    Solve();
}

void ClpInterface::SolveUsingPrimalSimplex() {
    SetPrimalPivotAlgorithm(PivotAlgorithm::Automatic);
    SetPresolvePasses(DefaultPresolvePasses);
    SetPrimalStartingBasis(StartingBasis::Default);
    SetPerturbation(DefaultPerturbation);
    MakePlusMinusOneMatrix(false);

    SetSolveType(SolveType::Primal);
    Solve();
}

void ClpInterface::SolveUsingPrimalIdiot() {
    SetPrimalPivotAlgorithm(PivotAlgorithm::Automatic);
    SetPresolvePasses(DefaultPresolvePasses);
    SetPrimalStartingBasis(StartingBasis::Idiot);
    SetPerturbation(DefaultPerturbation);
    MakePlusMinusOneMatrix(false);

    SetSolveType(SolveType::Primal);
    Solve();
}

void ClpInterface::SolveUsingEitherSimplex() {
    // Set special options that match Clp.exe -either. 16384 means be more
    // flexible, and ClpSimplex::housekeeping() modifies whether it factorizes.
    clp_->setMoreSpecialOptions(16384 | clp_->moreSpecialOptions());

    SetPresolvePasses(DefaultPresolvePasses);
    SetPerturbation(DefaultPerturbation);
    MakePlusMinusOneMatrix(true);

    SetSolveType(SolveType::Either);
    Solve();
}

void ClpInterface::SolveUsingBarrierMethod() {
    // Set special options that match Clp.exe -barrier. Which=4 sets Cholesky
    // type and barrier options. 2048 means native Cholesky factorization and
    // scaleBarrier = 2 (I do not know what it does). These settings arrive at
    // optimal result, although the solve time is not optimized.
    solve_options_->setSpecialOption(4, 2048);

    SetPresolvePasses(DefaultPresolvePasses);
    SetPerturbation(DefaultPerturbation);
    MakePlusMinusOneMatrix(false);

    SetSolveType(SolveType::Barrier);
    Solve();
}

void ClpInterface::StartModel() {
    // Load an empty model into the solver.
    clp_->loadProblem(
        CoinPackedMatrix(),
        nullptr, nullptr, nullptr,
        nullptr, nullptr, nullptr);
    clp_->dropNames();

    // The previous build objects should get destroyed automatically.
    row_build_object_ = CoinBuild(0);
    column_build_object_ = CoinBuild(1);

    // Clear the dense vectors.
    row_lower_bounds_.clear();
    row_upper_bounds_.clear();
    row_names_.clear();
    column_objectives_.clear();
    column_names_.clear();
}

int ClpInterface::AddVariable(std::string column_name, double lower_bound, double upper_bound) {
    int column_index = column_build_object_.numberColumns();
    assert(column_index == column_objectives_.size());
    assert(column_index == column_names_.size());
    column_build_object_.addColumn(0, nullptr, nullptr, lower_bound, upper_bound, 0.0);
    column_objectives_.push_back(0.0);
    column_names_.push_back(column_name);
    return column_index;
}

int ClpInterface::AddConstraint(std::string row_name, double lower_bound, double upper_bound) {
    int row_index = row_lower_bounds_.size();
    assert(row_index == row_upper_bounds_.size());
    assert(row_index == row_names_.size());
    row_lower_bounds_.push_back(lower_bound);
    row_upper_bounds_.push_back(upper_bound);
    row_names_.push_back(row_name);
    return row_index;
}

bool ClpInterface::AddCoefficients(int row_index, std::vector<int> &indices, std::vector<double> &elements) {
    if (indices.size() != elements.size()) {
        return false;
    }
    if (row_index < 0 || row_index >= row_lower_bounds_.size()) {
        return false;
    }
    // The row index should be the next row in the build object. We do not fill
    // empty rows or cannot go backwards.
    if (row_index != row_build_object_.numberRows()) {
        return false;
    }

    int count = indices.size();
    double lower_bound = row_lower_bounds_[row_index];
    double upper_bound = row_upper_bounds_[row_index];
    row_build_object_.addRow(count, indices.data(), elements.data(), lower_bound, upper_bound);
    return true;
}

bool ClpInterface::SetObjective(int column_index, double element) {
    if (column_index >= column_objectives_.size())
    {
        return false;
    }
    column_objectives_[column_index] = element;
    return true;
}

void ClpInterface::LoadModel() {
    clp_->addColumns(column_build_object_, false);
    clp_->addRows(row_build_object_, false);
    for (int index = 0; index < column_objectives_.size(); index++) {
        clp_->setObjectiveCoefficient(index, column_objectives_[index]);
    }
    clp_->copyRowNames(row_names_, 0, (int)row_names_.size());
    clp_->copyColumnNames(column_names_, 0, (int)column_names_.size());
}

} // namespace coinwrap