/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include <cstdio>
#include <cassert>
#include <iostream>

#include "clp_interface.h"

namespace coinwrap {

/// Initializes a new instance of the class.
ClpInterface::ClpInterface() :
    clp_(new ClpSimplex),
    solveOptions_(new ClpSolve) {}

/// Destroys an instance of the class.
ClpInterface::~ClpInterface() {}

/// Loads a model from an MPS file into the solver.
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

// Solves the optimization problem.
void ClpInterface::Solve() {

    // Set log level.
    CoinMessageHandler message_handler;
    clp_->passInMessageHandler(&message_handler);
    message_handler.setLogLevel(1, 1);
    clp_->setLogLevel(1);

    solveOptions_->setSpecialOption(3, 0);
    solveOptions_->setSolveType(ClpSolve::automatic);

    clp_->initialSolve(*solveOptions_);
}

} // namespace coinwrap