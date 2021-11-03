/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

/**
 * Clp model can be built using CoinBuild, CoinModel, CoinPackedMatrix and then
 * uploaded efficiently into ClpModel. CoinBuild is extremely lightweight and
 * best suited for adding entire column or row vectors. CoinModel has more
 * flexibility like adding an element at a time for the cost of being five times
 * slower than CoinBuild but four times faster than ClpModel. Add methods
 * require row and column indices and a separate hash table, like CoinModelHash.
 * This interface uses CoinModel.
 */

#ifndef COINWRAP_CLP_MODEL_BUILDER_H_
#define COINWRAP_CLP_MODEL_BUILDER_H_

#include <string>

#include "ClpModel.hpp"
#include "CoinModel.hpp"
#include "CoinModelUseful.hpp"

namespace coinwrap {

/*
 * Represents the interface to build a Clp model.
 */
class ClpModelBuilder {
 public:
    // Initializes a new instance of the class.
    explicit ClpModelBuilder();

    // Destroys an instance of the class.
    ~ClpModelBuilder();

    // Creates a variable with specified name, and upper and lower bounds.
    // It returns the column index assigned to the variable, or -1 if the name
    // is already in use.
    int AddVariable(std::string name, double lower_bound, double upper_bound);

    // Creates a constraint with specified name, and upper and lower bounds. It
    // returns the row index assigned to the constraint or -1 if the name is
    // already in use.
    int AddConstraint(std::string name, double lower_bound, double upper_bound);

    // Sets a coefficient in the constraint matrix by index.
    bool SetCoefficient(int row_index, int column_index, double value);

    // Sets a coefficient in the constraint matrix by name.
    bool SetCoefficient(std::string row_name, std::string column_name, double value);
    
    // Sets the objective.
    bool SetObjective(std::string column_name, double value);

    // Loads the model into the solver.
    void LoadModel(ClpModel *clp);

 private:
    // The model build object.
    CoinModel coin_model_;

    // The hash table mapping row and column names to indices.
    CoinModelHash row_hash_;
    CoinModelHash column_hash_;

    // The next row and column indices to assign to new rows and columns.
    int next_row_index_;
    int next_column_index_;
};

} // namespace coinwrap

#endif // COINWRAP_CLP_MODEL_BUILDER_H_
