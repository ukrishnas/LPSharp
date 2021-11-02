/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include "clp_model_builder.h"

#include "ClpModel.hpp"

namespace coinwrap {

ClpModelBuilder::ClpModelBuilder(ClpModel* model) :
    model_(model) {
}

} // namespace coinwrap