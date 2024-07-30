/*
 * Copyright (c) 2024 Umesh Krishnaswamy
 * This code is licensed under the terms of the MIT License.
 */

%module coinwrap_clp

%include "stdint.i"
%include "std_string.i"
%include "std_vector.i"

namespace std {
  %template (IntVector) vector<int>;
  %template (DoubleVector) vector<double>;
};

%{
#include "clp_interface.h"
%}

%include "clp_interface.h"
