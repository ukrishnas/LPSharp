/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include <cctype>
#include <iostream>
#include <string>
#include <string_view>

#include "clp_interface.h"

// Prints usage and exits.
void usage(const char * program_name) {
     char * usage_str = " filename recipe \n\
\n\
positional arguments: \n\
filename   MPS file name \n\
recipe     solve recipe, one of barrier, dual, either, primal ";

     std::cout << "usage: " << program_name << usage_str << std::endl;
     exit(1);
}

// Main program.
int main(int argc, const char *argv[]) {

     if (argc < 3) {
          usage(argv[0]);
     }

     std::string filename = argv[1];
     std::string recipe = argv[2];

     coinwrap::ClpInterface clp;

     bool status = clp.ReadMps(filename);
     std::cout << "Read " << filename << " status=" << status << std::endl;

     char recipe_key = std::tolower(std::string_view(recipe)[0]);
     switch (recipe_key) {
          case 'b':
               clp.SolveBarrierRecipe();
               break;
          case 'd':
               clp.SolveDualRecipe();
               break;
          case 'e':
               clp.SolveEitherRecipe();
               break;
          case 'p':
          default:
               clp.SolvePrimalRecipe();
               break;
     }

     return 0;
}
