/*
 * Copyright (c) 2024 Umesh Krishnaswamy
 * This code is licensed under the terms of the MIT License.
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
    std::cout << "Added constraints" << std::endl;

    int c1 = clp.AddVariable("c1", 0.0, 2.0);
    int c2 = clp.AddVariable("c2", 0.0, plusinf);
    int c3 = clp.AddVariable("c3", 0.0, 4.0);
    std::cout << "Added variables" << std::endl;

    std::vector<int> indices{c1, c2, c3};
    std::vector<double> coefficients1{1.0, 0, 1.0};
    std::cout << "Calling add coefficients for row " << r1 << std::endl;
    bool success = clp.AddCoefficients(r1, indices, coefficients1);
    assert(success);

    std::vector<double> coefficients2{1.0, -5.0, 1.0};
    std::cout << "Calling add coefficients for row " << r2 << std::endl;
    success = clp.AddCoefficients(r2, indices, coefficients2);
    assert(success);

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
    int number_rows = static_cast<int>(rows.size());

    std::vector<int> cols;
    cols.push_back(clp.AddVariable("c1", 0, plusinf));
    cols.push_back(clp.AddVariable("c2", 0.0, plusinf));
    cols.push_back(clp.AddVariable("c3", 0.0, plusinf));
    cols.push_back(clp.AddVariable("c4", 0, 20));
    cols.push_back(clp.AddVariable("c5", 0, 20));
    int number_cols = static_cast<int>(cols.size());

    std::cout << "Created " << number_rows << " rows and " << number_cols << " columns" << std::endl;

    double matrixElements[] = {
        8.0, 5.0, 4.0, 4.0, -4.0,
        8.0, 4.0, 5.0, 5.0, -5.0,
        1.0, -1.0, -1.0,
    };
    int nElements = sizeof(matrixElements)/sizeof(double);
    for (int row_index = 0; row_index < number_rows; row_index++) {
        std::vector<int> indices;
        std::vector<double> elements;
        for (int j = 0; j < number_cols; j++) {
            int idx = row_index * number_cols + j;
            if (idx < nElements) {
                indices.push_back(j);
                elements.push_back(matrixElements[idx]);
            }
        }
        bool success = clp.AddCoefficients(row_index, indices, elements);
        assert(success);
    }
    std::cout << "Set matrix coefficients" << std::endl;

    double objElements[] = { 1000, 400, 500, 10000, 10000 };
    for (int i = 0; i < number_cols; i++) {
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
