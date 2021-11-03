/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include <cctype>
#include <cstdlib>
#include <iomanip>
#include <iostream>
#include <string>
#include <string_view>
#include <vector>

#include "clp_interface.h"

// Prints usage and exits.
void usage(const char * program_name) {
    char * usage_str = " filename recipe \n\
\n\
positional arguments: \n\
filename   MPS file name \n\
method     solve method, one of dual, primal, either, barrier";

    std::cout << "usage: " << program_name << usage_str << std::endl;
    exit(1);
}

void CreateAndSolveModel(coinwrap::ClpInterface &clp);

inline void print_double(double value) {
    std::cout << std::setiosflags(std::ios::fixed | std::ios::showpoint) << std::setw(14) << value << " ";
}

// Main program.
int main(int argc, const char *argv[]) {
    if (argc < 3) {
        usage(argv[0]);
    }

    std::string filename = argv[1];
    std::string recipe = argv[2];

    coinwrap::ClpInterface clp;
    std::cout << "Successfully initialized clp interface" << std::endl; 

    int log_level = 1;
    clp.SetLogLevel(log_level);
    std::cout << "Set log level " << log_level << std::endl;

    // clp.SetMaximumSeconds(300);
    std::cout << "Set limits maximum seconds=" << clp.MaximumSeconds() << 
        " maximum iterations=" << clp.MaximumIterations() << std::endl;
    std::cout << "Set tolerances dual=" << clp.DualTolerance() << 
        " primal=" << clp.PrimalTolerance() << std::endl;

    bool status = clp.ReadMps(filename);
    if (!status) {
        std::cout << "ReadMps failed" << std::endl;
        return -1;
    }
    std::cout << "Read " << filename << " status=" << status << std::endl;

    if (recipe == "barrier") {
        clp.SolveUsingBarrierMethod();
    } else if (recipe == "duals") {
        clp.SolveUsingDualSimplex();
    } else if (recipe == "dualcrash") {
        clp.SolveUsingDualCrash();
    } else if (recipe == "either") {
        clp.SolveUsingEitherSimplex();
    } else if (recipe == "primals") {
        clp.SolveUsingPrimalSimplex();
    } else if (recipe == "primalidiot") {
        clp.SolveUsingPrimalIdiot();
    } else if (recipe == "makemodel") {
        CreateAndSolveModel(clp);
    } else {
        std::cout << "Unknown recipe " << recipe << std::endl;
        return -1;
    }

    // Print the solver settings.
    std::cout << "Recipe = " << recipe << std::endl;
    std::cout << "Solve type = " << clp.GetSolveType() << " dual starting basis = " << clp.DualStartingBasis() 
        << " primal starting basis = " << clp.PrimalStartingBasis() << std::endl;
    std::cout << "Presolve passes = " << clp.PresolvePasses() <<  ", perturbation = " << clp.Perturbation() << std::endl;
    printf("Status = %d Objective = %.10g iterations = %ld \n", clp.Status(), clp.ObjectiveValue(), clp.Iterations());
    std::cout << "Solve time milliseconds = " << clp.SolveTimeMs() << std::endl;

    int maxLines = 0;

    // Print the column solutions.
    std::vector<double> colSolution, reducedCost, objective;
    clp.PrimalColumnSolution(colSolution);
    clp.DualColumnSolution(reducedCost);
    clp.Objective(objective);
    int n = maxLines < objective.size() ? maxLines : objective.size();
    for (int i = 0; i < n; i++) {
        std::cout << std::setw(6) << i << " ";
        print_double(colSolution[i]);
        print_double(reducedCost[i]);
        print_double(objective[i]);
        std::cout << std::endl;
        if (i == n - 1) std::cout << "--------------------------------------" << std::endl;
    }
    
    // Print the row solutions.
    std::vector<double> rowActivity, rowPrice;
    clp.PrimalRowSolution(rowActivity);
    clp.DualRowSolution(rowPrice);
    int m = maxLines < rowActivity.size() ? maxLines : rowActivity.size();
    for (int i = 0; i < m; i++) {
        std::cout << std::setw(6) << i << " ";
        print_double(rowActivity[i]);
        print_double(rowPrice[i]);
        std::cout << std::endl;
        if (i == n - 1) std::cout << "--------------------------------------" << std::endl;
    }
    
    return 0;
}

// Creates and solves a model.
void CreateAndSolveModel(coinwrap::ClpInterface &clp) {
    double plusinf = std::numeric_limits<double>::infinity();
    double minusinf = -1.0 * std::numeric_limits<double>::infinity();

    int c1 = clp.AddVariable("c1", 0.0, 2.0);
    int c2 = clp.AddVariable("c2", 0.0, plusinf);
    int c3 = clp.AddVariable("c3", 0.0, 4.0);

    int r1 = clp.AddConstraint("r1", 0.0, 1.0);
    int r2 = clp.AddConstraint("r2", 0.0, 0.0);

    clp.SetCoefficient(r1, c1, 1.0);
    clp.SetCoefficient(r1, c2, 0);
    clp.SetCoefficient(r1, c3, 1.0);

    clp.SetCoefficient(r2, c1, 1.0);
    clp.SetCoefficient(r2, c2, -5.0);
    clp.SetCoefficient(r2, c3, 1.0);

    clp.SetObjective(c1, 1.0);
    clp.SetObjective(c2, 0);
    clp.SetObjective(c3, 4.0);

    clp.LoadModel();

    clp.SolveUsingDualSimplex();
}