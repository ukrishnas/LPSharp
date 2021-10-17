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
        /// <param name="boundsName">The optional bounds name.</param>
        /// <param name="rhsName">The optional right hand side name.</param>
        /// <param name="rangesName">The optional ranges name.</param>
        /// <returns>True if the model was loaded successfully, false otherwise.</returns>
        bool Load(
            LPModel model,
            string boundsName = null,
            string rhsName = null,
            string rangesName = null);

        /// <summary>
        /// Solves the model.
        /// </summary>
        void Solve();
    }
}
