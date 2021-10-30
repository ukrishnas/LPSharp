/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include "clp_interface.h"

#include <cstdio>
#include <cassert>
#include <iostream>

#include "ClpDualRowSteepest.hpp"
#include "ClpPrimalColumnSteepest.hpp"
#include "ClpPEDualRowSteepest.hpp"
#include "ClpPEPrimalColumnSteepest.hpp"
#include "ClpSimplex.hpp"
#include "ClpSolve.hpp"

namespace coinwrap {

// The default number of presolve passes.
#define DefaultPresolvePasses 10

// The default perturbation value.
#define DefaultPerturbation 50

// Initializes a new instance of the ClpInterface with default values. The
// values used here are the defaults.
ClpInterface::ClpInterface() :
    clp_(new ClpSimplex),
    solve_options_(new ClpSolve),
    message_handler_(new CoinMessageHandler),
    presolve_passes_(DefaultPresolvePasses),
    dual_pivot_algorithm_(PivotAlgorithm::Automatic),
    primal_pivot_algorithm_(PivotAlgorithm::Automatic),
    positive_edge_psi_(0.5) {
    
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

bool ClpInterface::ReadMps(std::string filename) {
    FILE *fp;
    fp = fopen(filename.c_str(), "r");
    if (fp) {
        fclose(fp);
    } else {
        std::cout << "Unable to open " << filename << std::endl;
        return false;
    }

    // Always keep names and do not ignore errors.
    std::cout << "Reading mps file " << filename << std::endl;
    int status = clp_->readMps(filename.c_str(), true, false);
    return status == 0;
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

void ClpInterface::SetPresolve(bool enable, int passes) {
    if (!enable) {
        solve_options_->setPresolveType(ClpSolve::presolveOff, 0);
    } else {
        solve_options_->setPresolveType(ClpSolve::presolveOn, passes);

        // This setting is important. It fixes infeasibility by allowing
        //  presolve transforms to arbitrarily ignore infeasibility and set
        //  arbitrary feasible bounds. This coupled with perturbation setting
        //  causes the model to be perturbed in primal and dual to fix
        //  infeasibility.
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
        solve_options_->setSpecialOption(0, 2);
    } else {
        // Invalid or unsupported basis method.
        return false;
    }
    
    return true;
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
        solve_options_->setSpecialOption(1, 2);
    } else if (basis == StartingBasis::Sprint) {
        solve_options_->setSpecialOption(1, 3);
    } else {
        return false;
    }

    return true;
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
    std::cout << "Solve type " << solve_options_->getSolveType() << std::endl;
    std::cout << "Special options " << clp_->specialOptions() << std::endl;
    std::cout << "More special options " << clp_->moreSpecialOptions() << std::endl;
    for (int i = 0; i < 3; i++) {
        std::cout << "Independent option " << i << " = " << solve_options_->independentOption(i) << std::endl;
    }
    std::cout << "Perturbation " << Perturbation() << std::endl;
    std::cout << "Presolve type=" << solve_options_->getPresolveType() << " passes=" << solve_options_->getPresolvePasses() << std::endl;

    clp_->initialSolve(*solve_options_);
}

void ClpInterface::SolveUsingDualSimplex() {
    SetDualPivotAlgorithm(PivotAlgorithm::Automatic);
    SetPresolve(true, DefaultPresolvePasses);
    SetDualStartingBasis(StartingBasis::Crash);
    SetPerturbation(DefaultPerturbation);
    MakePlusMinusOneMatrix(false);

    SetSolveType(SolveType::Dual);
    Solve();
}

void ClpInterface::SolveUsingPrimalSimplex() {
    SetPrimalPivotAlgorithm(PivotAlgorithm::Automatic);
    SetPresolve(true, DefaultPresolvePasses);
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

    SetPresolve(true, DefaultPresolvePasses);
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

    SetPresolve(true, DefaultPresolvePasses);
    SetPerturbation(DefaultPerturbation);
    MakePlusMinusOneMatrix(false);

    SetSolveType(SolveType::Barrier);
    Solve();
}

} // namespace coinwrap