// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlopSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    /// <summary>
    /// Represents the interface implementation for GLOP solver.
    /// </summary>
    public class GlopSolver : OrtoolsSolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlopSolver"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        public GlopSolver(string key)
            : base(key, "GLOP_LINEAR_PROGRAMMING")
        {
        }

        /// <summary>
        /// Sets solver-specific parameters.
        /// </summary>
        public void SetSpecificParameters()
        {
            // Parameters need to be a protocol buffer serialized string of type GlopParameters.
            // The SetSolverSpecificParametersAsString() converts the string back into GlopParameters
            // and updates solver parameters.. Some settings are shown below. See also
            // ortools/glop/parameters.proto.
            //
            // InitialBasisHeuristic initial_basis = NONE, BIXBY, TRIANGULAR (default), MAROS.
            // PricingRule feasibility_rule = DANTZIG, STEEPEST_EDGE (default), DEVEX.
            // PricingRule optimization_rule.
            //
            // This function is a placeholder until the rest of the mechanics are figured out.
        }
    }
}
