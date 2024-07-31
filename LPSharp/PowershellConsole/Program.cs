// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.PowershellConsole
{
    using LPSharp.LPDriver.Model;
    using LPSharp.Powershell;

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
