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
#include "ortools/glop/lp_solver.h"
#include "ortools/glop/parameters.pb.h"
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

    // Sets underlying solver parameters.
    void SetGlopParameters();

    // Prints the underlying solver parameters.
    void PrintGlopParameters();

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
  LOG(INFO) << "Solving model with parameters ...";
  absl::Time start_time = absl::Now();
  MPSolver::ResultStatus result_status = solver_->Solve(*mp_parameters_);
  absl::Duration solve_time = absl::Now() - start_time;

  // Print result.
  LOG(INFO) << "Result status = " << result_status;
  LOG(INFO) << "Solve time = " << solve_time / absl::Milliseconds(1);
  if (result_status == MPSolver::OPTIMAL) {
    LOG(INFO) << "Objective value = " << solver_->Objective().Value();
    LOG(INFO) << "Iterations = " << solver_->iterations();
  }
  PrintGlopParameters();
}

void GlopSolver::SetGlopParameters() {
  glop::LPSolver * glop_lpsolver = static_cast<glop::LPSolver *>(solver_->underlying_solver());
  glop::GlopParameters* parameters = glop_lpsolver->GetMutableParameters();

  // parameters->set_scaling_method(glop::GlopParameters::EQUILIBRATION);
  // parameters->set_initial_basis(glop::GlopParameters::NONE);
  // parameters->set_use_dual_simplex(true);
  parameters->set_perturb_costs_in_dual_simplex(true);
  parameters->set_relative_cost_perturbation(1e3);
  parameters->set_relative_max_cost_perturbation(1e5);
  // parameters->set_dual_feasibility_tolerance(1e-3);
  // parameters->set_primal_feasibility_tolerance(1e-3);
  // parameters->set_use_scaling(false);

  std::string message;
  google::protobuf::TextFormat::PrintToString(*parameters, &message);
  glop::GlopParameters newparams;
  google::protobuf::TextFormat::MergeFromString(message, &newparams);
  solver_->SetSolverSpecificParametersAsString(message);
}

void GlopSolver::PrintGlopParameters() {
  glop::LPSolver * glop_lpsolver = static_cast<glop::LPSolver *>(solver_->underlying_solver());
  glop::GlopParameters parameters(glop_lpsolver->GetParameters());

  LOG(INFO) << "Use dual simplex = " << parameters.use_dual_simplex();
  LOG(INFO) << "Initial basis heuristic = " << parameters.initial_basis();
  LOG(INFO) << "Feasibility pricing rule = " << parameters.feasibility_rule();
  LOG(INFO) << "Optimization pricing rule = " << parameters.optimization_rule();
  LOG(INFO) << "Scaling algorithm = " << parameters.scaling_method();
  LOG(INFO) << "Use scaling = " << parameters.use_scaling();
  LOG(INFO) << "Primal feasibility tolerance = " << parameters.primal_feasibility_tolerance();
  LOG(INFO) << "Dual feasibility tolerance = " << parameters.dual_feasibility_tolerance();
  LOG(INFO) << "Solution feasibility tolerance = " << parameters.solution_feasibility_tolerance();
  LOG(INFO) << "Perturb costs in dual simplex = " << parameters.perturb_costs_in_dual_simplex();
  LOG(INFO) << "Relative cost perturbation = " << parameters.relative_cost_perturbation();
  LOG(INFO) << "Relative max cost perturbation = " << parameters.relative_max_cost_perturbation();
}

} // namespace glop_solver
} // namespace operations_research

// Prints usage and exits.
void usage(const char * program_name) {
  char * usage_str = " filename  method\n\
\n\
positional arguments: \n\
filename   MPS file name \n\
method     Solve method: primal dual \n\
";

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
  absl::SetFlag(&FLAGS_logtostderr, true);
  absl::SetFlag(&FLAGS_log_prefix, true);
  absl::SetFlag(&FLAGS_v, 0);

  // Initialize solver.
  operations_research::glop_solver::GlopSolver glop_solver;
  
  // Set time limit.
  glop_solver.SetTimeLimit(30);

  // Set the LP algorithm. Insted of doing it this way, I am trying to do it
  // with glop parameters.
  std::string algorithm = argv[2];
  glop_solver.SetLPAlgorithm(algorithm);

  // Set GLOP specific parameters. The values are hard-coded in the method.
  glop_solver.SetGlopParameters();

  // Load and solve the model.
  std::string filename = argv[1];
  glop_solver.LoadAndSolve(filename);
}