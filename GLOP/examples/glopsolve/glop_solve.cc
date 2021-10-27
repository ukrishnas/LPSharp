/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

#include <filesystem>
#include <iostream>
#include <string>
#include <string_view>

#include "absl/flags/flag.h"
#include "absl/time/time.h"
#include "ortools/base/logging.h"
#include "ortools/linear_solver/linear_solver.h"
#include "ortools/linear_solver/linear_solver.pb.h"
#include "ortools/lp_data/mps_reader.h"

namespace operations_research {
namespace glop_solver {

class GlopSolver {
  public:
    // Initializes a new instance of this class.
    explicit GlopSolver();

    // Destroys an instance of this class.
    ~GlopSolver() {}

    // Loads a model from file and solves it.
    void LoadAndSolve(std::string filename);

    // Sets the time limit.
    void SetTimeLimit(int value) {
      absl::Duration time_limit = absl::Seconds(value);
      solver_->SetTimeLimit(time_limit);
      LOG(INFO) << "Set solve time limit to " << time_limit;
    }

    // Sets the LP algorithm.
    void SetLPAlgorithm(std::string value) {
      char key = std::tolower(std::string_view(value)[0]);
      if (key == 'd') {
        mp_parameters_->SetIntegerParam(
          MPSolverParameters::IntegerParam::LP_ALGORITHM,
          (int)MPSolverParameters::LpAlgorithmValues::DUAL);
        LOG(INFO) << "Set LP algorithm to dual";
      }
    }

  private:
    // The solver.
    std::unique_ptr<MPSolver> solver_;

    // Solver parameters.
    std::unique_ptr<MPSolverParameters> mp_parameters_;
};

GlopSolver::GlopSolver() :
    solver_(MPSolver::CreateSolver("GLOP")),
    mp_parameters_(new MPSolverParameters) {
}

void GlopSolver::LoadAndSolve(std::string filename) {
  // Read the MPS model.
  glop::MPSReader reader;
  MPModelProto input_model;
  reader.ParseFile(filename, &input_model);

  // Load model into solver.
  std::string error_message;
  MPSolverResponseStatus status = solver_->LoadModelFromProto(input_model, &error_message);
  if (status != MPSolverResponseStatus::MPSOLVER_MODEL_IS_VALID) {
    LOG(FATAL) << "Load model status " << status << " is invalid";
  }
  LOG(INFO) << "Loaded model from " << filename;

  // Solve.
  LOG(INFO) << "Solving model ...";
  MPSolver::ResultStatus result_status = solver_->Solve(*mp_parameters_);

  // Print result.
  LOG(INFO) << "Result status = " << result_status;
  LOG(INFO) << "Solve time = " << solver_->wall_time();
  if (result_status == MPSolver::OPTIMAL) {
    LOG(INFO) << "Objective value = " << solver_->Objective().Value();
    LOG(INFO) << "Iterations = " << solver_->iterations();
  }
}

} // namespace glop_solver
} // namespace operations_research

// Prints usage and exits.
void usage(const char * program_name) {
  char * usage_str = " filename algorithm \n\
\n\
positional arguments: \n\
filename   MPS file name \n\
algorithm  solve method, one of dual, primal";

  std::cout << "usage: " << program_name << usage_str << std::endl;
  exit(1);
}

int main(int argc, char *argv[]) {
  if (argc < 3) {
    usage(argv[0]);
  }

  // Enable Google logging which is used by the solver. Set logging to current
  // working directory and turn on verbosity 1 to show solver logs. It is also
  // possible to set GLOP parameter to enable logging.
  google::InitGoogleLogging(argv[0]);
  absl::SetFlag(&FLAGS_log_dir, std::filesystem::current_path().string());
  absl::SetFlag(&FLAGS_logtostderr, false);
  absl::SetFlag(&FLAGS_log_prefix, true);
  absl::SetFlag(&FLAGS_v, 1);

  // Initialize solver.
  operations_research::glop_solver::GlopSolver glop_solver;
  
  // Set time limit to 90 seconds.
  glop_solver.SetTimeLimit(30);

  // Parse the recipe argument and set LP algorithm.
  std::string algorithm = argv[2];
  glop_solver.SetLPAlgorithm(algorithm);

  // Load and solve the model.
  std::string filename = argv[1];
  glop_solver.LoadAndSolve(filename);
}