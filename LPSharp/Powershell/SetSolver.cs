﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetSolver.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.Powershell
{
    using System.Management.Automation;
    using LPSharp.LPDriver.Contract;
    using LPSharp.LPDriver.Model;

    /// <summary>
    /// Invokes the solver.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "Solver")]
    public class SetSolver : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the solver key.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the solver type to create.
        /// </summary>
        [Parameter]
        [Alias("Create")]
        public SolverType? CreateSolverType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make this solver the default
        /// for all solver operations.
        /// </summary>
        [Parameter]
        public SwitchParameter Default { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to read default parameters from LP driver.
        /// </summary>
        [Parameter]
        public SwitchParameter DefaultParameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clear the solver state. This resets
        /// the extracted model and clears the loaded model. This does not affect parameters.
        /// </summary>
        [Parameter]
        public SwitchParameter Clear { get; set; }

        /// <summary>
        /// Gets or sets the solver parameters as semicolon separated key-value pairs.
        /// </summary>
        [Parameter]
        [Alias("Parameters")]
        public string ParametersText { get; set; }

        /// <summary>
        /// Gets or sets the parameters file. Solver parameters are set using this file.
        /// </summary>
        [Parameter]
        [Alias("ParamsFile")]
        public string ParametersFile { get; set; }

        /// <summary>
        /// The process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            ILPInterface solver;

            if (this.CreateSolverType != null)
            {
                var solverType = this.CreateSolverType.Value;
                solver = this.LPDriver.CreateSolver(this.Key, solverType);
                if (solver == null)
                {
                    this.WriteHost($"Could not create solver type={solverType}");
                    return;
                }

                this.WriteHost($"Solver {solver} created");
            }
            else
            {
                solver = this.LPDriver.GetSolver(this.Key);
                if (solver == null)
                {
                    this.WriteHost($"Solver {this.Key} key not found");
                    return;
                }
            }

            var solver2 = solver as LPSolverAbstract;

            if (this.Default)
            {
                this.LPDriver.DefaultSolverKey = this.Key;
                this.WriteHost($"Solver {this.LPDriver.DefaultSolverKey} is default solver");
            }

            if (this.Clear)
            {
                solver.Clear();
                this.WriteHost($"Solver {solver2.Key} state and model cleared");
            }

            if (this.DefaultParameters && this.LPDriver.SolverParameters != null)
            {
                solver.SetParameters(this.LPDriver.SolverParameters);
                this.WriteHost("Set solver parameters from LP driver");
            }

            bool hasParamsFile = !string.IsNullOrEmpty(this.ParametersFile);
            if (hasParamsFile)
            {
                var solverParameters = Utility.ReadParameters(this.ParametersFile);
                if (solverParameters == null)
                {
                    this.WriteHost($"Solver parameters file {this.ParametersFile} not found or invalid");
                    return;
                }

                solver.SetParameters(solverParameters);
                this.WriteHost($"Set solver parameters from {this.ParametersFile}");
            }

            bool hasParameters = !string.IsNullOrEmpty(this.ParametersText);
            if (hasParameters)
            {
                var solverParameters = Utility.ParseParameters(this.ParametersText);
                if (solverParameters == null)
                {
                    this.WriteHost($"Solver parameters text is null or invalid");
                    return;
                }

                solver.SetParameters(solverParameters);
                this.WriteHost($"Set solver {solver2.Key} parameters from {this.ParametersText}");
            }
        }
    }
}
