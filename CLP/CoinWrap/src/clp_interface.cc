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
#include "ClpSimplex.h"
#include "ClpSolve.h"

namespace coinwrap {

// The default number of presolve passes.
#define DefaultPresolvePasses 5

// Initializes a new instance of the ClpInterface with default values. The
// values used here are the defaults.
ClpInterface::ClpInterface() :
    clp_(new ClpSimplex),
    solve_options_(new ClpSolve),
    message_handler_(new CoinMessageHandler),
    presolve_passes_(DefaultPresolvePasses),
    dual_pivot_algorithm_(PivotAlgorithm.Automatic),
    primal_pivot_algorithm_(PivotAlgorithm.Automatic),
    positive_edge_psi_(0.5) {
    
    // Initialize the message handler to no logging.
    clp_->passInMessageHandler(message_handler_);
    message_handler.setLoglevel(0);
    clp_->setLogLevel(0);
    clp_->setPrefix(false);
}

ClpInterface::~ClpInterface() {}

// Sets the log level. Values range [0, 4] with 0 being none and 4 being verbose.
void SetLogLevel(int level) {
    if (level >= 0 && level <= 4) {
        // In the message handler set log level, first parameter stands for the
        // facility, and 1 refers to the solver.
        messageHandler_->setLogLevel(1, level);

        // Set the log level in the solver.
        clp_->setLogLevel(level);
    }
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
    if (pivot_algorithm == PivotAlgorithm.Automatic) {
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

bool ClpInterface::SetPresolve(bool enable, int passes) {
    if (!enable) {
        solve_options_->setPresolveType(PresolveType::presolveOff, 0);
    } else {
        if (passes > 0) {
            solve_options_->setPresolveType(PresolveType::presolveNumber, passes);
        } else {
            solve_options_->setPresolveType(PresolveType::presolveOn, DefaultPresolvePasses);
        }
    }
}

void ClpInterface::MakePlusMinusOneMatrix(bool enable) {
    // The special option arguments which=3 is for controlling whether to make
    // plus minus 1-matrix. Value=0 means make the matrix, and 1 means do not
    // make the matrix.
    if (enable) {
        solve_options_->setSpecialOptions(3, 1);
    } else {
        solve_options_->setSpecialOptions(3, 0);
    }
}

bool ClpInterface::SetDualStartingBasis(StartingBasis basis) {
    // The special option arguments are which and value. Which is always zero
    // for setting dual starting basis. The values are from comments in
    // ClpSolve.hpp and code in ClpSolve.cpp. On average, dual simplex seems to
    // perform better with no basis.
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
    // The special option arguments are which and value. Which is always zero
    // for setting primal starting basis. The values are from comments in
    // ClpSolve.hpp and code in ClpSolve.cpp. This method does not offer all the
    // options of Clp. It does not set extra options for sprint or crash, or
    // support the additional combinations of using crash, idiot, and sprint.
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

// Solves the continuous relaxation of the current model using the dual steepest
// edge algorithm. The time and iterations may be affected by settings such as
// presolve, crash, and and dual and primal tolerances.
void ClpInterface::SolveUsingDualSimplex() {
    // Set special options that match Clp.exe -dual switch. Which=0 and value=2
    // means startup in dual with initiative about idiot but no crash. On average, dual
    // simplex seems to perform better without basis.
    solve_options_->setSpecialOption(0, 2);

    // Set solve method.
    solve_options_->setSolveType(ClpSolve::useDual);

    clp_->initialSolve(*solve_options_);
}

// Solves the continuous relaxation of the current model using the primal
// algorithm. The time and iterations may be affected by settings such as
// presolve, scaling, crash and also by column selection  method, infeasibility
// weight and dual and primal tolerances.
void ClpInterface::SolveUsingPrimallSimplex() {
    // Set special options that match Clp.exe -primal switch. Which=1, value=7
    // means startup in primal using initiative but no crash.
    solve_options_->setSpecialOption(1, 7);

    // Set solve method.
    solve_options_->setSolveType(ClpSolve::usePrimalorSprint);

    clp_->initialSolve(*solve_options_);
}

// Solves the optimization problem using either dual pr primal based on dubious
// (author's words) analysis of the model.
void ClpInterface::SolveUsingEitherSimplex() {
    // Set special options that match Clp.exe -either switch. This switch
    // does not set any special options but sets more special options.
    // 16384 means be more flexible in initialSolve.
    clp_->setMoreSpecialOptions(16384 | clp_->moreSpecialOptions());

    // Set solve method. The method is called automatic.
    solve_options_->setSolveType(ClpSolve::automatic);

    // clp_->setPerturbation(50);
    clp_->initialSolve(*solve_options_);
}

void ClpInterface::SolveUsingBarrierMethod() {
    // Set special options that match Clp.exe -barrier switch.
    // Which 3 = Do not make plus minus 1-matrix.
    // Which 4 = cholesky type and barrier options. I think they are calculated values.
    solve_options_->setSpecialOption(3, 1);
    solve_options_->setSpecialOption(4, 2048);

    // Set solve method.
    solve_options_->setSolveType(ClpSolve::useBarrier);

    clp_->initialSolve(*solve_options_);
}

} // namespace coinwrap