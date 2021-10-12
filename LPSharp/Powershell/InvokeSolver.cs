// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Invokes an LP solver.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "Solver")]
    public class InvokeSolver : LPCmdlet
    {
        /// <summary>
        /// The process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            TestSolver.TestGlop();
        }
    }
}
