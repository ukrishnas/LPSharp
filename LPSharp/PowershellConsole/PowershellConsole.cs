// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowershellConsole.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.PowershellConsole
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;

    using Microsoft.LPSharp.Powershell;

    using PowerShell = System.Management.Automation.PowerShell;

    /// <summary>
    /// The PowerShell console.
    /// </summary>
    public class PowershellConsole
    {
        /// <summary>
        /// The instance mutex.
        /// </summary>
        private readonly object instanceMutex = new object();

        /// <summary>
        /// The run space.
        /// </summary>
        private readonly Runspace runspace;

        /// <summary>
        /// The host.
        /// </summary>
        private readonly PowershellHost host;

        /// <summary>
        /// The running PowerShell.
        /// </summary>
        private PowerShell runningPowershell;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowershellConsole"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="prompt">The prompt.</param>
        public PowershellConsole(string name, string prompt)
        {
            this.Name = name;
            this.Prompt = prompt;

            this.host = new PowershellHost(this);
            InitialSessionState initSession = InitialSessionState.CreateDefault();

            // Setup a console host readline function like the real console host does.
            // This allows us to load PsReadline if present, or otherwise fall back to Console.Readline.
            initSession.Commands.Add(new SessionStateFunctionEntry("PSConsoleHostReadline", "[System.Console]::ReadLine()"));
            initSession.ImportPSModule(new[] { "PsReadline" });
            Assembly assembly = Assembly.GetAssembly(typeof(LPCmdlet));

            initSession.ImportPSModule(new[] { new Uri(assembly.Location).AbsolutePath });
            initSession.AuthorizationManager = new AuthManager(name);
            this.runspace = RunspaceFactory.CreateRunspace(this.host, initSession);
            this.host.PushRunspace(this.runspace);
            this.runspace.Open();
        }

        /// <summary>
        /// Gets the prompt.
        /// </summary>
        public string Prompt { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether should exit.
        /// </summary>
        public bool ShouldExit { get; set; }

        /// <summary>
        /// Gets or sets the exit code.
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Injects a variable.
        /// </summary>
        /// <param name="variableName">The variable name.</param>
        /// <param name="value">The value.</param>
        public void InjectVariable(string variableName, object value)
        {
            var command = new PSCommand();
            command.AddCommand("Set-Variable");
            command.AddParameter("Name", variableName);
            command.AddParameter("Value", value);
            command.AddParameter("Option", ScopedItemOptions.None);
            this.ExecuteCommands(new[] { command });
        }

        /// <summary>
        /// Runs until exit.
        /// </summary>
        public void Run()
        {
            Console.CancelKeyPress += this.HandleControlC;
            Console.TreatControlCAsInput = false;

            while (!this.ShouldExit)
            {
                this.host.UI.Write(ConsoleColor.Cyan, Console.BackgroundColor, string.Format("\n{0}> ", this.Prompt));
                string cmd = this.host.UI.ReadLine();
                this.RunCommand(cmd);
            }

            Environment.Exit(this.ExitCode);
        }

        /// <summary>
        /// Runs a command.
        /// </summary>
        /// <param name="command">The command to run.</param>
        public void RunCommand(string command)
        {
            try
            {
                this.ExecuteHelper(command, null);
            }
            catch (RuntimeException rte)
            {
                this.ReportException(rte);
            }
        }

        /// <summary>
        /// Reports exceptions to the console
        /// </summary>
        /// <param name="e">The exception.</param>
        private void ReportException(Exception e)
        {
            if (e == null)
            {
                return;
            }

            var icer = e as IContainsErrorRecord;
            object error = icer != null ? icer.ErrorRecord : new ErrorRecord(e, "Host.ReportException", ErrorCategory.NotSpecified, null);

            lock (this.instanceMutex)
            {
                this.runningPowershell = PowerShell.Create();
            }

            this.runningPowershell.Runspace = this.runspace;

            try
            {
                this.runningPowershell.AddScript("$input").AddCommand("out-string");

                // Do not merge errors, this function will swallow errors.
                var inputCollection = new PSDataCollection<object> { error };
                inputCollection.Complete();
                Collection<PSObject> result = this.runningPowershell.Invoke(inputCollection);

                if (result.Count <= 0)
                {
                    return;
                }

                var str = result[0].BaseObject as string;
                if (!string.IsNullOrEmpty(str))
                {
                    // Remove \r\n that is added by Out-string.
                    this.host.UI.WriteErrorLine(str.Substring(0, str.Length - 2));
                }
            }
            finally
            {
                lock (this.instanceMutex)
                {
                    this.runningPowershell.Dispose();
                    this.runningPowershell = null;
                }
            }
        }

        /// <summary>
        /// Executes commands.
        /// </summary>
        /// <param name="commands">The commands.</param>
        private void ExecuteCommands(IEnumerable<PSCommand> commands)
        {
            lock (this.instanceMutex)
            {
                this.runningPowershell = PowerShell.Create();
            }

            try
            {
                this.runningPowershell.Runspace = this.runspace;

                foreach (PSCommand command in commands)
                {
                    this.runningPowershell.Commands = command;
                    this.runningPowershell.Invoke();
                }
            }
            finally
            {
                lock (this.instanceMutex)
                {
                    this.runningPowershell.Dispose();
                    this.runningPowershell = null;
                }
            }
        }

        /// <summary>
        /// The execute helper.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="input">The input.</param>
        private void ExecuteHelper(string command, object input)
        {
            // Ignore empty command lines.
            if (string.IsNullOrEmpty(command))
            {
                return;
            }

            lock (this.instanceMutex)
            {
                this.runningPowershell = PowerShell.Create();
            }

            this.runningPowershell.Runspace = this.runspace;

            try
            {
                this.runningPowershell.AddScript(command);

                this.runningPowershell.AddCommand("out-default");
                this.runningPowershell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

                if (input != null)
                {
                    this.runningPowershell.Invoke(new[] { input });
                }
                else
                {
                    this.runningPowershell.Invoke();
                }
            }
            finally
            {
                lock (this.instanceMutex)
                {
                    this.runningPowershell.Dispose();
                    this.runningPowershell = null;
                }
            }
        }

        /// <summary>
        /// Handles control-C note: eaten by <c>PSReadline</c>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleControlC(object sender, ConsoleCancelEventArgs e)
        {
            try
            {
                lock (this.instanceMutex)
                {
                    if (this.runningPowershell != null && this.runningPowershell.InvocationStateInfo.State == PSInvocationState.Running)
                    {
                        this.runningPowershell.Stop();
                    }
                }

                e.Cancel = true;
            }
            catch (Exception ex)
            {
                this.host.UI.WriteErrorLine(ex.ToString());
            }
        }
    }
}
