// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILPInterface.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Represents the interface to an LP solver.
    /// </summary>
    public interface ILPInterface
    {
        /// <summary>
        /// Loads a model.
        /// </summary>
        /// <param name="model">The solver model.</param>
        /// <returns>True if the model was loaded successfully, false otherwise.</returns>
        bool Load(LPModel model);

        /// <summary>
        /// Solves the model.
        /// </summary>
        void Solve();
    }
}
