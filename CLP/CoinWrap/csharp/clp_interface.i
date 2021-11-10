/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

%module coinwrap_clp

%include "stdint.i"
%include "std_vector.i"

namespace std {
    %template (DoubleVector) vector<double>;
};

%include "clp_interface.h"
