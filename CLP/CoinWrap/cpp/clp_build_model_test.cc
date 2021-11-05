/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#ifdef NDEBUG
#undef NDEBUG
#endif // To enable assert.
#include <cassert>
#include <iostream>
#include <vector>

#include "clp_interface.h"

// Tests Clp interface methods to create a model and solve it.
void CreateAndSolveTestModel1() {
    std::cout << "TEST CreateAndSolveTestModel1" << std::endl;
    coinwrap::ClpInterface clp;

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

    std::cout << "Created model with 2 constraints and 3 variables" << std::endl;
    clp.LoadModel();
    clp.WriteMps("test1.mps");
    std::cout << "Wrote model to test1.mps" << std::endl;

    clp.SolveUsingDualSimplex();
    double result = clp.ObjectiveValue();
    std::cout << "Solved module using dual simplex, objective = " << result << std::endl;

    assert(fabs(result - 2) < 1e-7);
}

// Tests Clp interface methods to create a model and solve it.
void CreateAndSolveTestModel2() {
    std::cout << "TEST CreateAndSolveTestModel2" << std::endl;
    coinwrap::ClpInterface clp;

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

// Main program.
int main(int argc, const char* argv[]) {
    CreateAndSolveTestModel1();
    CreateAndSolveTestModel2();
}
