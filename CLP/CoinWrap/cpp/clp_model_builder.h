/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

/**
 * This is a wrapper around the Clp methods to build a model. First some
 * terminology, ClpModel is the model class, ClpSimplex is the solver class and
 * a subclass of ClpModel. CoinBuild is more efficient at adding rows or columns
 * and it is 20 times faster to build the model in CoinBuild and then sending
 * the build object to ClpModel. Alternatively a PackedMatrix can be used to
 * create the matrix and then loaded into the model.
 * 
 * multiple ways to build a model. It is more efficient to add rows or columns
 * into a build object (CoinBuild) and add the entire build object into Clp
 * model. This approach can be 20 times faster than adding rows or columns one
 * at a time into the model.
 * 
 * Note that ClpModel is a base class for ClpSimplex. The
 * solver class (ClpSimplex) is a subclass of the model class (ClpModel).  Using CoinBuild you can add rows or columns,
 * and then load the entire build object efficiently. or column at a time. This requires the
 * column to be fully formed. This wrapperCoinModel and ClpModel to incrementally add
 * variables and constraints. The recommended way to add column vectors into
 * CoinModel and then adding the entire CoinModel into the solver model is the 
 *
 *  which form the columns and rows of the model. Normal Clp methods Columns
 * Represents an interface to build a Clp model. Cl similar to the
 * MPSolverInterface in OR-Tools. The difference is that no state is maintained
 * this class and it uses native CLP model building tools like CoinBuild.
 *
 *  Methods still to be implemented:
 *   - addColumn
 *   - modifyCoefficient
 *   - setColumnBounds
 *   - setColumnName
 *   - setObjectiveCoefficient
 *   - setObjectiveOffset
 *   - setRowBounds
 *   - setRowName
 */

#ifndef COINWRAP_CLPMODEL_H_
#define COINWRAP_CLPMODEL_H_

#include <string>
#include <map>

#include "ClpModel.hpp"
#include "CoinBuild.hpp"

namespace coinwrap {

/*
 * Represents the interface to build a Clp model.
 */
class ClpModelBuilder {
 public:
    // Initializes a new instance of the class.
    explicit ClpModelBuilder(ClpModel *model);

    // Destroys an instance of the class.
    ~ClpModelBuilder();

    // Create a variable with specified name, and upper and lower bound.
    // It returns the variable index. This index can be used in add column calls.
    int MakeRealVariable(std::string name, double lowerBound, double upperBound);

    // Creates a constraint.
    double * MakeConstraint(double lowerBound, double upperBound);

    // Sets a coefficient in the constraint.
    void SetCoefficient(double* constraint, std::string column, double value);

    // Creates an objective.
    void SetObjective(std::string name);

    void SetObjective(double* objective, std::string column, double value);

 private:
    ClpModel *model_;
}

} // namespace coinwrap

#endif // COINWRAP_CLPMODEL_H_
