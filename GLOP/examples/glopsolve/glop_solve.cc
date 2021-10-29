/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Apache License Version 2.0.
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

/**
 * Standalone executable for GLOP solver. Please see usage() below for example usage.
 */

#include <fstream>
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

ABSL_FLAG(std::string, mpsfile, "", "MPS file name");
ABSL_FLAG(int32_t, timelimit, -1, "Solve time limit in seconds");
ABSL_FLAG(std::string, lpalgorithm, "primal", "Common parameters LP solver algorithm primal or dual");
ABSL_FLAG(bool, presolve, true, "Common parameters presolve on or off");
ABSL_FLAG(std::string, paramsfile, "", "GLOP parameters file name");

namespace operations_research {
namespace glop_solver {

/*
 * Represents a standalone Glop solver. It can read a model in MPS proto format
 * and solve with custom common and solver-specific parameters.
 */
class GlopSolver {
  public:
    // Initializes a new instance of this class.
    explicit GlopSolver();

    // Destroys an instance of this class.
    ~GlopSolver() {}

    // Loads a model from file and solves it.
    void LoadAndSolve(std::string model_filename);

    // Sets underlying solver parameters from a file.
    void SetGlopParametersFromFile(std::string params_filename);

    // Prints the underlying solver parameters.
    void PrintGlopParameters();

    // Sets the solve time limit.
    void SetTimeLimit(int value) {
      absl::Duration time_limit = absl::Seconds(value);
      solver_->SetTimeLimit(time_limit);
      LOG(INFO) << "Set solve time limit to " << time_limit;
    }

    // Sets the LP algorithm in the common parameters. GLOP only supports primal
    // and dual. It does not support barrier method.
    void SetLPAlgorithm(std::string value) {
      char key = std::tolower(std::string_view(value)[0]);
      if (key == 'd') {
        mp_parameters_->SetIntegerParam(
          MPSolverParameters::IntegerParam::LP_ALGORITHM,
          (int)MPSolverParameters::LpAlgorithmValues::DUAL);
        LOG(INFO) << "Set LP algorithm to dual";
      } else if (key == 'p') {
        mp_parameters_->SetIntegerParam(
          MPSolverParameters::IntegerParam::LP_ALGORITHM,
          (int)MPSolverParameters::LpAlgorithmValues::PRIMAL);
          LOG(INFO) << "Set LP algorithm to primal";
      }
    }

    // Sets the parameter indicating whether to perform presolve.
    void SetPresolve(bool value) {
      if (value) {
        mp_parameters_->SetIntegerParam(
          MPSolverParameters::IntegerParam::PRESOLVE,
          (int)MPSolverParameters::PresolveValues::PRESOLVE_ON);
      } else {
        mp_parameters_->SetIntegerParam(
          MPSolverParameters::IntegerParam::PRESOLVE,
          (int)MPSolverParameters::PresolveValues::PRESOLVE_OFF);
      }
      LOG(INFO) << "Set presolve to " << value;       
    }

  private:
    // The solver.
    std::unique_ptr<MPSolver> solver_;

    // The common solver parameters.
    std::unique_ptr<MPSolverParameters> mp_parameters_;
};

GlopSolver::GlopSolver() :
    solver_(MPSolver::CreateSolver("GLOP")),
    mp_parameters_(new MPSolverParameters) {
}

void GlopSolver::LoadAndSolve(std::string model_filename) {
  // Read the MPS model. Explicitly check if the file is present, since I am not
  // certain if the MPS reader returns an invalid status if file does not exist.
  if (FILE *file = fopen(model_filename.c_str(), "r")) {
    fclose(file);
  } else {
    LOG(FATAL) << "Unable to open MPS file " << model_filename;
  }
  glop::MPSReader reader;
  MPModelProto input_model;
  absl::Status read_status = reader.ParseFile(model_filename, &input_model);
  if (!read_status.ok()) {
    LOG(FATAL) << "Unable to read MPS file " << model_filename << ". Status " << read_status;
  }

  // Load the model into the solver.
  std::string error_message;
  MPSolverResponseStatus load_status = solver_->LoadModelFromProto(input_model, &error_message);
  if (load_status != MPSolverResponseStatus::MPSOLVER_MODEL_IS_VALID) {
    LOG(FATAL) << "Load model status " << load_status << " is invalid. " << error_message;
  }
  LOG(INFO) << "Loaded model from " << model_filename;

  // Solve. Note that solver_->wall_time() returns the duration since
  // construction, which is not what we need. We need time for the solve call
  // only.
  LOG(INFO) << "Solving model with parameters ...";
  absl::Time start_time = absl::Now();
  MPSolver::ResultStatus result_status = solver_->Solve(*mp_parameters_);
  absl::Duration solve_time = absl::Now() - start_time;

  // Print result and parameters.
  LOG(INFO) << "Result status = " << result_status;
  LOG(INFO) << "Solve() time milliseconds = " << solve_time / absl::Milliseconds(1);
  if (result_status == MPSolver::OPTIMAL) {
    LOG(INFO) << "Objective value = " << solver_->Objective().Value();
    LOG(INFO) << "Iterations = " << solver_->iterations();
  }
  PrintGlopParameters();
}

void GlopSolver::SetGlopParametersFromFile(std::string params_filename) {
  // The solver specific parameters should be set as a string inside the solver.
  // Otherwise, the Solve() method will overwrite the common and solver-specific
  // parameters with defaults even when it is called with no parameters.
  std::ifstream in(params_filename.c_str(), std::ifstream::in);
  if (!in.good()) {
    LOG(FATAL) << "Unable to open GLOP parameters file " << params_filename;
  }
  std::ostringstream sstr;
  sstr << in.rdbuf();
  std::string parameters_proto = sstr.str();

  glop::GlopParameters newparams;
  if (!google::protobuf::TextFormat::MergeFromString(parameters_proto, &newparams)) {
    LOG(WARNING) << "Solver may not be able to extract solver specific parameters from text";
  }

  solver_->SetSolverSpecificParametersAsString(parameters_proto);
}

void GlopSolver::PrintGlopParameters() {
  glop::LPSolver * glop_lpsolver = static_cast<glop::LPSolver *>(solver_->underlying_solver());
  glop::GlopParameters parameters(glop_lpsolver->GetParameters());

  LOG(INFO) << "Use dual simplex = " << parameters.use_dual_simplex();
  LOG(INFO) << "Initial basis heuristic = " << glop::GlopParameters::InitialBasisHeuristic_Name(parameters.initial_basis());
  LOG(INFO) << "Feasibility pricing rule = " << glop::GlopParameters::PricingRule_Name(parameters.feasibility_rule());
  LOG(INFO) << "Optimization pricing rule = " << glop::GlopParameters::PricingRule_Name(parameters.optimization_rule());
  LOG(INFO) << "Use scaling = " << parameters.use_scaling();
  LOG(INFO) << "Scaling algorithm = " << glop::GlopParameters::ScalingAlgorithm_Name(parameters.scaling_method());
  LOG(INFO) << "Primal feasibility tolerance = " << parameters.primal_feasibility_tolerance();
  LOG(INFO) << "Dual feasibility tolerance = " << parameters.dual_feasibility_tolerance();
  LOG(INFO) << "Solution feasibility tolerance = " << parameters.solution_feasibility_tolerance();
  LOG(INFO) << "Perturb costs in dual simplex = " << parameters.perturb_costs_in_dual_simplex();
  LOG(INFO) << "Relative cost perturbation = " << parameters.relative_cost_perturbation();
  LOG(INFO) << "Relative max cost perturbation = " << parameters.relative_max_cost_perturbation();
}

} // namespace glop_solver
} // namespace operations_research

std::string usage() {
  std::string usage_str = std::string("glopsolve");
  usage_str += " minimal usage: -mpsfile <MPS file> -lpalgorithm <primal|dual> -timelimit <int>";
  usage_str += "\n\
\n\
Load and solve an MPS file with default parameters. \n\
\n\
glopsolve -mpsfile <MPS file> \n\
\n\
Load and solve with a 30 second solve time limit and dual simplex algorithm. \n\
\n\
glopsolve -mpsfile <MPS file> -timelimit 30 -lpalgorithm dual \n\
\n\
Turn on verbose logging and log to a file in the current directory (default \n\
is the system log directory). Turn off logging to standard error which is on by default. \n\
\n\
glopsolve --mpsfile <MPS file> --nologtosterr -v=1 -log_dir=. \n\
\n\
Set solver-specific parameters in command line. \n\
glopsolve -mpsfile <MPS file> --glop_params=\"perturb_costs_in_dual_simplex: 1 optimization_rule: 2\" \n\
";

  return usage_str;
}

int main(int argc, char *argv[]) {
  // Parse command line arguments. Turn on logging to stderr by default so that
  // some output is shown on screen. User can turn it off when doing verbose
  // solver logging.
  absl::SetFlag(&FLAGS_logtostderr, true);
  absl::SetProgramUsageMessage(usage());
  absl::ParseCommandLine(argc, argv);
  std::string mpsfile = absl::GetFlag(FLAGS_mpsfile);
  if (mpsfile.empty()) {
    LOG(FATAL) << "Please specify file name with -mpsfile";
  }

  // Initialize logging which is used by the solver and this module.
  google::InitGoogleLogging(argv[0]);

  // Initialize the solver.
  operations_research::glop_solver::GlopSolver glop_solver;
  
  // Set the time limit.
  int timelimit = absl::GetFlag(FLAGS_timelimit);
  if (timelimit != -1) {
    glop_solver.SetTimeLimit(timelimit);
  }

  // Set common parameters.
  glop_solver.SetLPAlgorithm(absl::GetFlag(FLAGS_lpalgorithm));
  glop_solver.SetPresolve(absl::GetFlag(FLAGS_presolve));

  // Set GLOP specific parameters from file. To set a parameters by string, user
  // can use the flag --glop_params. This file based flag is to read multiple
  // more complex parameters. See glop/parameters.proto for the available
  // parameters.
  std::string paramsfile = absl::GetFlag(FLAGS_paramsfile);
  if (!paramsfile.empty()) {
    glop_solver.SetGlopParametersFromFile(paramsfile);
  }

  // Load and solve the model.
  glop_solver.LoadAndSolve(absl::GetFlag(FLAGS_mpsfile));
}