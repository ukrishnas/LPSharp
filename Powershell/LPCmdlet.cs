// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPCmdlet.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// The linear programming cmdlet.
    /// </summary>
    public class LPCmdlet : PSCmdlet
    {
        /// <summary>
        /// The LP engine variable name.
        /// </summary>
        public const string LPDriverVariableName = "global:LPSharpPowershell_LPDriver";

        /// <summary>
        /// Indicates whether the application base has been initialized.
        /// </summary>
        private static bool appBaseInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPCmdlet"/> class.
        /// </summary>
        protected LPCmdlet()
        {
            InitializeAppBase();
        }

        /// <summary>
        /// Gets or sets the LP engine.
        /// </summary>
        [Parameter]
        public LPDriver LPDriver { get; set; }

        /// <summary>
        /// Gets a value indicating whether the -verbose flag was supplied.
        /// </summary>
        protected bool VerboseOutput { get; private set; }

        /// <summary>
        /// Begins processing, called once per pipeline.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.VerboseOutput = this.MyInvocation.BoundParameters.ContainsKey("Verbose");
            this.SetLPDriver();
        }

        /// <summary>
        /// Writes an enumerable to the output stream, if written directly the underlying object is written (list/array)
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        protected void WriteEnumerable(IEnumerable enumerable)
        {
            foreach (object e in enumerable)
            {
                if (this.Stopping)
                {
                    break;
                }

                this.WriteObject(e);
            }
        }

        /// <summary>
        /// Writes a string to the host console
        /// </summary>
        /// <param name="msgFormat">The message format.</param>
        /// <param name="objects">The parameters for the message.</param>
        protected void WriteHost(string msgFormat, params object[] objects)
        {
            this.Host.UI.WriteLine(string.Format(msgFormat, objects));        
        }

        /// <summary>
        /// Initializes the application base directory.
        /// </summary>
        private static void InitializeAppBase()
        {
            if (!appBaseInitialized)
            {
                // We don't have any locking here, so it's possible for this code to execute
                // more than once, but that's acceptable because that won't change the end result.
                appBaseInitialized = true;

                // This is required because certain resource loaders use the AppDomain root for plugin loading but in PowerShell,
                // the application domain root is the location of the powershell.exe console.
                var assemblyDir = new Uri(Assembly.GetAssembly(typeof(LPCmdlet)).Location);
                AppDomain.CurrentDomain.SetData("APPBASE", Path.GetDirectoryName(assemblyDir.AbsolutePath));
            }
        }

        /// <summary>
        /// Sets the LP engine property if it isn't set explicitly by the runtime.
        /// </summary>
        private void SetLPDriver()
        {
            if (this.LPDriver != null)
            {
                return;
            }

            object driver = this.SessionState.PSVariable.GetValue(LPDriverVariableName);
            if (driver != null)
            {
                this.LPDriver = driver as LPDriver;
                if (this.LPDriver == null)
                {
                    throw new InvalidDataException(
                        string.Format(
                            "{0} was expected to be of type ILPDriver but was {1} instead, please recreate",
                            LPDriverVariableName,
                            driver.GetType().FullName));
                }
            }
            else
            {
                this.LPDriver = new LPDriver();
                this.SessionState.PSVariable.Set(LPDriverVariableName, this.LPDriver);
            }
        }
    }
}
