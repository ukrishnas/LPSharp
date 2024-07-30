/*
 * Copyright (c) 2024 Umesh Krishnaswamy
 * This code is licensed under the terms of the MIT License.
 */

#include <cctype>
#include <filesystem>
#include <iomanip>
#include <iostream>
#include <string>
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

// Prints a formatted double value.
inline void print_double(double value) {
    std::cout << std::setiosflags(std::ios::fixed | std::ios::showpoint) << std::setw(14) << value << " ";
}

// Prints row and column solutions.
void PrintSolution(coinwrap::ClpInterface &clp, int max_lines) {
    if (max_lines == 0) {
        return;
    }

    // Print the column solutions.
    std::cout << "Column index    column solution   reduced cost   objective" << std::endl;
    std::vector<double> colSolution, reducedCost, objective;
    clp.PrimalColumnSolution(colSolution);
    clp.DualColumnSolution(reducedCost);
    clp.Objective(objective);
    int n = max_lines < objective.size() ? max_lines : objective.size();
    for (int i = 0; i < n; i++) {
        std::cout << std::setw(6) << i << " ";
        print_double(colSolution[i]);
        print_double(reducedCost[i]);
        print_double(objective[i]);
        std::cout << std::endl;
    }
    std::cout << "--------------------------------------" << std::endl;

    // Print the row solutions.
    std::cout << "Row index    row activity   row price" << std::endl;
    std::vector<double> rowActivity, rowPrice;
    clp.PrimalRowSolution(rowActivity);
    clp.DualRowSolution(rowPrice);
    int m = max_lines < rowActivity.size() ? max_lines : rowActivity.size();
    for (int i = 0; i < m; i++) {
        std::cout << std::setw(6) << i << " ";
        print_double(rowActivity[i]);
        print_double(rowPrice[i]);
        std::cout << std::endl;
    }
    std::cout << "--------------------------------------" << std::endl;
}

// Main program.
int main(int argc, const char* argv[]) {
    namespace fs = std::filesystem;
    fs::path basepath;
    if (argc < 3) {
        usage(argv[0]);
    }

    std::string filename = argv[1];
    std::string recipe = argv[2];

    coinwrap::ClpInterface clp;
    std::cout << "Successfully initialized clp interface" << std::endl; 

    int log_level = 3;
    clp.SetLogLevel(log_level);
    std::cout << "Set log level " << log_level << std::endl;

    // clp.SetMaximumSeconds(300);
    std::cout << "Set limits maximum seconds=" << clp.MaximumSeconds() << 
        " maximum iterations=" << clp.MaximumIterations() << std::endl;
    std::cout << "Set tolerances dual=" << clp.DualTolerance() << 
        " primal=" << clp.PrimalTolerance() << std::endl;

    bool status = clp.ReadMps(filename.c_str());
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
    PrintSolution(clp, 10);

    return 0;
}