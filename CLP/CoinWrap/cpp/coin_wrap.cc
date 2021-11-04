/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include <cassert>
#include <cctype>
#include <cstdlib>
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

void PrintSolution(coinwrap::ClpInterface &clp, int max_lines);
void CreateAndSolveTestModel1(coinwrap::ClpInterface &clp);
void CreateAndSolveTestModel2(coinwrap::ClpInterface &clp);

// Main program.
int main(int argc, const char* argv[]) {
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

    if (recipe == "test1") {
        CreateAndSolveTestModel1(clp);
    } else if (recipe == "test2") {
        CreateAndSolveTestModel2(clp);
    } else {
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

// Creates and solves test model 1.
void CreateAndSolveTestModel1(coinwrap::ClpInterface &clp) {
    double plusinf = std::numeric_limits<double>::infinity();
    double minusinf = -1.0 * std::numeric_limits<double>::infinity();

    clp.StartModel();
    int r1 = clp.AddConstraint("r1", 2.0, plusinf);
    int r2 = clp.AddConstraint("r2", 1.0, 1.0);

    int c1 = clp.AddVariable("c1", 0.0, 2.0);
    int c2 = clp.AddVariable("c2", 0.0, plusinf);
    int c3 = clp.AddVariable("c3", 0.0, 4.0);

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
    clp.WriteMps("test1.mps");
    clp.SolveUsingDualSimplex();

    double result = clp.ObjectiveValue();
    assert(fabs(result - 2) < 1e-7);
}

// Creates and solves a test model 2.
void CreateAndSolveTestModel2(coinwrap::ClpInterface &clp) {
    double plusinf = std::numeric_limits<double>::infinity();
    double minusinf = -1.0 * std::numeric_limits<double>::infinity();

    clp.StartModel();

    std::vector<int> rows;
    rows.push_back(clp.AddConstraint("r1", 20, plusinf));
    rows.push_back(clp.AddConstraint("r2", minusinf, 30));
    rows.push_back(clp.AddConstraint("r3", 8, 8));
    int numberRows = rows.size();

    std::vector<int> cols;
    cols.push_back(clp.AddVariable("c1", 0, plusinf));
    cols.push_back(clp.AddVariable("c2", 0.0, plusinf));
    cols.push_back(clp.AddVariable("c3", 0.0, plusinf));
    cols.push_back(clp.AddVariable("c4", 0, 20));
    cols.push_back(clp.AddVariable("c5", 0, 20));
    int numberCols = cols.size();

    std::cout << "Created " << numberRows << " rows and " << numberCols << " columns" << std::endl;

    double matrixElements[] = {
        8.0, 5.0, 4.0, 4.0, -4.0,
        8.0, 4.0, 5.0, 5.0, -5.0,
        1.0, -1.0, -1.0,
    };
    int nElements = 13;
    for (int i = 0; i < numberRows; i++) {
        for (int j = 0; j < numberCols; j++) {
            int idx = i * numberCols + j;
            if (idx < nElements) {
                clp.SetCoefficient(rows[i], cols[j], matrixElements[idx]);
            }
        }
    }
    std::cout << "Set matrix coefficients" << std::endl;

    double objElements[] = { 1000, 400, 500, 10000, 10000 };
    for (int i = 0; i < numberCols; i++) {
        clp.SetObjective(cols[i], objElements[i]);
    }
    std::cout << "Set objective coefficients" << std::endl;

    clp.LoadModel();
    std::cout << "Loaded model" << std::endl;
    clp.WriteMps("test2.mps");
    std::cout << "Wrote MPS" << std::endl;
    clp.SolveUsingDualSimplex();

    double result = clp.ObjectiveValue();
    assert(fabs(result - 76000) < 1e-7);
}

inline void print_double(double value) {
    std::cout << std::setiosflags(std::ios::fixed | std::ios::showpoint) << std::setw(14) << value << " ";
}

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