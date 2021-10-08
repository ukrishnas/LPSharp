// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthManager.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.PowershellConsole
{
    using System;
    using System.Management.Automation;
    using System.Management.Automation.Host;

    /// <summary>
    /// The authorization manager.
    /// </summary>
    public class AuthManager : AuthorizationManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthManager"/> class.
        /// </summary>
        /// <param name="shellId">The shell identifier.</param>
        public AuthManager(string shellId)
            : base(shellId)
        {
        }

        /// <summary>
        /// Indicates whether a certain command should be run.
        /// </summary>
        /// <param name="commandInfo">The command information.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="host">The host.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>True when command may run.</returns>
        protected override bool ShouldRun(CommandInfo commandInfo, CommandOrigin origin, PSHost host, out Exception reason)
        {
            // Return true here to get unrestricted execution policy.
            reason = null;
            return true;
        }
    }
}
