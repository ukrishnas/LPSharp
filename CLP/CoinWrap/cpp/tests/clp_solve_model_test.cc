/*
 * Copyright (c) 2024 Umesh Krishnaswamy
 * This code is licensed under the terms of the MIT License.
 */

#ifdef NDEBUG
#undef NDEBUG
#endif // To enable assert.
#include <cassert>
#include <cctype>
#include <cstdlib>
#include <filesystem>
#include <iomanip>
#include <iostream>
#include <string>
#include <tuple>
#include <vector>

#include "clp_interface.h"

// Tolerance for comparing double precision values.
const double kTolerance = 1e-4;

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
    int number_cols = static_cast<int>(objective.size());
    int n = max_lines < number_cols ? max_lines : number_cols;
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
    int number_rows = static_cast<int>(rowActivity.size());
    int m = max_lines < number_rows ? max_lines : number_rows;
    for (int i = 0; i < m; i++) {
        std::cout << std::setw(6) << i << " ";
        print_double(rowActivity[i]);
        print_double(rowPrice[i]);
        std::cout << std::endl;
    }
    std::cout << "--------------------------------------" << std::endl;
}

// Loads and solves an MPS model using a specific solver method.
void LoadAndSolve(std::string filename, std::string recipe, double expect_objective, int expect_iterations) {
    std::cout << "TEST LoadAndSolve filename = " << filename << " recipe = " << recipe << std::endl;
    
    FILE *fp;
    fp = fopen(filename.c_str(), "r");
    if (fp) {
        fclose(fp);
    } else {
        assert(false && "Test file not found");
    }

    coinwrap::ClpInterface clp;
    std::cout << "Successfully initialized clp interface" << std::endl; 

    int log_level = 3;
    clp.SetLogLevel(log_level);
    std::cout << "Log level = " << log_level << std::endl;

    clp.SetMaximumSeconds(60);
    std::cout << "Solver limits maximum seconds = " << clp.MaximumSeconds() << 
        " maximum iterations = " << clp.MaximumIterations() << std::endl;
    std::cout << "Set tolerances dual=" << clp.DualTolerance() << 
        " primal=" << clp.PrimalTolerance() << std::endl;

    bool status = clp.ReadMps(filename.c_str());
    std::cout << "Read " << filename << " status = " << status << std::endl;
    assert(status && "ReadMps failed");

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
        assert(false && "Unknown recipe");
    }

    // Print the solver settings.
    std::cout << "Recipe = " << recipe << std::endl;
    std::cout << "Solve type = " << clp.GetSolveType() << " dual starting basis = " << clp.DualStartingBasis() 
        << " primal starting basis = " << clp.PrimalStartingBasis() << std::endl;
    std::cout << "Presolve passes = " << clp.PresolvePasses() <<  ", perturbation = " << clp.Perturbation() << std::endl;
    printf("Status = %d Objective = %.14g iterations = %ld \n", clp.Status(), clp.ObjectiveValue(), clp.Iterations());
    std::cout << "Solve time milliseconds = " << clp.SolveTimeMs() << std::endl;
    PrintSolution(clp, 10);

    assert(fabs(clp.ObjectiveValue() - expect_objective) < kTolerance);
    assert(expect_iterations == clp.Iterations());
}

// Main program.
int main(int argc, const char* argv[]) {
    namespace fs = std::filesystem;
    fs::path basepath;
    if (argc > 1) {
        basepath /= argv[1];
    }

    std::string path_80bau38 = (basepath / "80bau38.mps").string();

    std::vector<std::tuple<std::string, std::string, double, int>> tests;
    tests.push_back(std::make_tuple(path_80bau38, "duals", 987224.1924, 5556));
    tests.push_back(std::make_tuple(path_80bau38, "dualcrash", 987224.1924, 6438));
    tests.push_back(std::make_tuple(path_80bau38, "primals",  987224.1924, 5398));
    tests.push_back(std::make_tuple(path_80bau38, "primalidiot", 987224.1924, 7026));
    tests.push_back(std::make_tuple(path_80bau38, "either", 987224.1924, 3253));
    tests.push_back(std::make_tuple(path_80bau38, "barrier", 987224.1924, 381));

    for(auto it = tests.begin(); it != tests.end(); it++) {
        std::string filename = std::get<0>(*it);
        std::string recipe = std::get<1>(*it);
        double expect_objective = std::get<2>(*it);
        int expect_iterations = std::get<3>(*it);

        LoadAndSolve(filename, recipe, expect_objective, expect_iterations);
    }
}