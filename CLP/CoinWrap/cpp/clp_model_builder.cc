/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include "clp_model_builder.h"

#include <cassert>

#include "ClpModel.hpp"
#include "CoinModel.hpp"
#include "CoinModelUseful.hpp"

namespace coinwrap {

ClpModelBuilder::ClpModelBuilder() :
    next_row_index_(0),
    next_column_index_(0) {
}

ClpModelBuilder::~ClpModelBuilder() {}

int ClpModelBuilder::AddVariable(std::string name, double lower_bound, double upper_bound) {
    const char * column_name = name.c_str();
    int which_column = column_hash_.hash(column_name);
    if (which_column != -1) {
        return -1;
    }

    column_hash_.addHash(next_column_index_, column_name);
    which_column = next_column_index_++;
    assert(which_column == column_hash_->hash(column_name));

    coin_model_.setColumnName(which_column, column_name);
    coin_model_.setColumnBounds(which_column, lower_bound, upper_bound);
    return which_column;
}

int ClpModelBuilder::AddConstraint(std::string name, double lower_bound, double upper_bound) {
    const char * row_name = name.c_str();
    int which_row = row_hash_.hash(row_name);
    if (which_row != -1) {
        return -1;
    }

    row_hash_.addHash(next_row_index_, row_name);
    which_row = next_row_index_++;
    assert(which_row == row_hash_.hash(row_name));

    coin_model_.setRowName(which_row, row_name);
    coin_model_.setRowBounds(which_row, lower_bound, upper_bound);
    return which_row;
}

bool ClpModelBuilder::SetCoefficient(int row_index, int column_index, double value) {
    if (row_index == -1 || row_index >= next_row_index_) {
        return false;
    }
    if (column_index == -1 || column_index >= next_column_index_) {
        return false;
    }
    coin_model_.setElement(row_index, column_index, value);
    return true;
}

bool ClpModelBuilder::SetCoefficient(std::string row_name, std::string column_name, double value) {
    int row_index = row_hash_.hash(row_name.c_str());
    int column_index = column_hash_.hash(column_name.c_str());
    if (row_index == -1 || column_index == -1) {
        return false;
    }
    return SetCoefficient(row_index, column_index, value);
}

bool ClpModelBuilder::SetObjective(std::string column_name, double value) {
    int column_index = column_hash_.hash(column_name.c_str());
    if (column_index == -1) {
        return false;
    }
    coin_model_.setColumnObjective(column_index, value);
    return true;
}

void ClpModelBuilder::LoadModel(ClpModel* clp) {
    clp->loadProblem(coin_model_, false);
}

} // namespace coinwrap