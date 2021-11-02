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
    // clp.SetMaximumIterations(60000);
    std::cout << "Set limits maximum seconds=" << clp.MaximumSeconds() << 
        " maximum iterations=" << clp.MaximumIterations() << std::endl;

    // clp.SetDualTolerance(1e-6);
    // clp.SetPrimalTolerance(1e-6);
    std::cout << "Set tolerances dual=" << clp.DualTolerance() << 
        " primal=" << clp.PrimalTolerance() << std::endl;

    bool status = clp.ReadMps(filename);
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
    } else {
        std::cout << "Unknown recipe " << recipe << std::endl;
    }

    printf("Status = %d Objective = %.10g iterations = %ld \n",
        clp.Status(), clp.ObjectiveValue(), clp.Iterations());

    int maxLines = 0;

    // Print the column solution, reduced cost, and objective.
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
