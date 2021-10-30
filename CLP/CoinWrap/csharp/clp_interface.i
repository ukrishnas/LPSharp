/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * This code is licensed under the terms of the Eclipse Public License (EPL).
 * 
 * Authors
 * 
 * Umesh Krishnaswamy
 */

%include "enums.swg"
%include "stdint.i"

%{
    #include "clp_innterface.h"
%}

// Allow C# types to be partial classes so they can be extended using C# instead
// of typemap(cscode)
%typemap(csclassmodifiers) SWIGTYPE "public partial class"

%ignoreall

// Pivot algorithm types.
%unignore coinwrap::PivotAlgorithm::Automatic;
%unignore coinwrap::PivotAlgorithm::ExactDevex;
%unignore coinwrap::PivotAlgorithm::PartialDevex;
%unignore coinwrap::PivoAlgorithm::SteepestEdge;
%unignore coinwrap::PivotAlgorithm::PositiveEdgeSteepest;

// Starting basis types.
%unignore coinwrap::StartingBasis::Default;
%unignore coinwrap::StartingBasis::AllSlack;
%unignore coinwrap::StartingBasis::Crash;
%unignore coinwrap::StartingBasis::Idiot;
%unignore coinwrap::StartingBasis::Sprint;

// Solve types.
%unignore coinwrap::SolveType::Dual;
%unignore coinwrap::SolveType::Primal;
%unignore coinwrap::SolveType::Either;
%unignore coinwrap::SolveType::Barrier;

// Constructors.
%unignore coinwrap::ClpInterface::ClpInterface;
%unignore coinwrap::ClpInterface::~ClpInterface;

// Methods to set and get simple parameters.
%unignore coinwrap::ClpInterface::PresolvePasses;
%unignore coinwrap::ClpInterface::SetPresolvePasses;
%unignore coinwrap::ClpInterface::OptimizeDirection;
%unignore coinwrap::ClpInterface::DualTolerance;
%unignore coinwrap::ClpInterface::SetDualTolerance;
%unignore coinwrap::ClpInterface::PrimalTolerance;
%unignore coinwrap::ClpInterface::SetPrimalTolerance;
%unignore coinwrap::ClpInterface::PrimalWeight;
%unignore coinwrap::ClpInterface::SetPrimalWeight;
%unignore coinwrap::ClpInterface::PositiveEdgePsi;
%unignore coinwrap::ClpInterface::SetPositiveEdgePsi;
%unignore coinwrap::ClpInterface::Perturbation;
%unignore coinwrap::ClpInterface::SetPerturbation;

// Methods to set limits.
%unignore coinwrap::ClpInterface::MaximumIterations;
%unignore coinwrap::ClpInterface::SetMaximumIterations;
%unignore coinwrap::ClpInterface::MaximumSeconds;
%unignore coinwrap::ClpInterface::SetMaximumSeconds;

// Methods for setting more involved parameters.
%unignore coinwrap::ClpInterface::SetLogLevel;
%unignore coinwrap::ClpInterface::Reset;
%unignore coinwrap::ClpInterface::ReadMps;
%unignore coinwrap::ClpInterface::SetPrimalPivotAlgorithm;
%unignore coinwrap::ClpInterface::SetDualPivotAlgorithm;
%unignore coinwrap::ClpInterface::SetPresolve;
%unignore coinwrap::ClpInterface::MakePlusMinusOneMatrix;
%unignore coinwrap::ClpInterface::SetDualStartingBasis;
%unignore coingwrap::ClpInterface::SetPrimalStartingBasis;

// Solve methods.
%unignore coinwrap::ClpInterface::Solve;
%unignore coinwrap::ClpInterface::SolveUsingDualSimplex;
%unignore coinwrap::ClpInterface::SolveUsingPrimalSimplex;
%unignore coinwrap::ClpInterface::SolveUsingEitherSimplex;
%unignore coinwrap::ClpInterface::SolveUsingBarrierMethod;

%include "clp_interface.h"
%unignoreall