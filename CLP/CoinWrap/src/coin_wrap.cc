/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include <iostream>

#include "clp_interface.h"

int main(int argc, const char *argv[]) {

     coinwrap::ClpInterface clp;

     if (argc < 2) {
          std::cout << "Please pass a MPS file name. Usage " << argv[0] << " <filename>" << std::endl;
          return -1;
     }

     std::string filename(argv[1]);
     bool status = clp.ReadMps(filename);
     std::cout << "Read " << filename << " status=" << status << std::endl;

     clp.Solve();

     return 0;
}
