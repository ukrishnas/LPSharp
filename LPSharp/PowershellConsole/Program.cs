// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.PowershellConsole
{
    using Microsoft.LPSharp.LPDriver.Model;
    using Microsoft.LPSharp.Powershell;

    /// <summary>
    /// Represents the main program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Runs the console
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            // Create the listener and run it - this never returns...
            var driver = new LPDriver();
            var listener = new PowershellConsole("psconsole", "LPSharp");
            listener.InjectVariable(LPCmdlet.LPDriverVariableName, driver);
            if (args.Length > 0)
            {
                listener.RunCommand(string.Join(" ", args));
            }
            else
            {
                listener.Run();
            }
        }
    }
}
