// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowershellHost.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.PowershellConsole
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Management.Automation.Host;
    using System.Management.Automation.Runspaces;

    /// <summary>
    /// This is a sample implementation of the PSHost abstract class for console applications. Not all members are
    /// implemented. Those that are not implemented throw a NotImplementedException exception or return nothing. Code
    /// originally from MSDN as a sample, adapted to fit style etc.
    /// </summary>
    internal class PowershellHost : PSHost, IHostSupportsInteractiveSession
    {
        /// <summary>
        /// The instance ID.
        /// </summary>
        private readonly Guid instanceId = Guid.NewGuid();

        /// <summary>
        /// The original culture info.
        /// </summary>
        private readonly CultureInfo originalCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

        /// <summary>
        /// The original UI culture info.
        /// </summary>
        private readonly CultureInfo originalUiCultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;

        /// <summary>
        /// The program.
        /// </summary>
        private readonly PowershellConsole program;

        /// <summary>
        /// The <c>runspace</c> stack.
        /// </summary>
        private readonly Stack<Runspace> runspaceStack = new Stack<Runspace>();

        /// <summary>
        /// The host user interface.
        /// </summary>
        private readonly HostUserInterface hostUserInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowershellHost"/> class. 
        /// </summary>
        /// <param name="program">A reference to the host application object.</param>
        public PowershellHost(PowershellConsole program)
        {
            this.program = program;
            this.hostUserInterface = new HostUserInterface(this);
        }

        /// <summary>
        /// Gets a value indicating whether a runspace has been pushed.
        /// </summary>
        public bool IsRunspacePushed
        {
            get
            {
                return this.runspaceStack.Count > 1;
            }
        }

        /// <summary>
        /// Gets the current runspace.
        /// </summary>
        public Runspace Runspace
        {
            get
            {
                return this.runspaceStack.Peek();
            }
        }

        /// <inheritdoc />
        public override CultureInfo CurrentCulture
        {
            get
            {
                return this.originalCultureInfo;
            }
        }

        /// <inheritdoc />
        public override CultureInfo CurrentUICulture
        {
            get
            {
                return this.originalUiCultureInfo;
            }
        }

        /// <inheritdoc />
        public override Guid InstanceId
        {
            get
            {
                return this.instanceId;
            }
        }

        /// <inheritdoc />
        public override string Name
        {
            get
            {
                return "EmbeddedConsoleHost";
            }
        }

        /// <inheritdoc />
        public override PSHostUserInterface UI
        {
            get
            {
                return this.hostUserInterface;
            }
        }

        /// <inheritdoc />
        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        /// <summary>
        /// Pushes a runspace<.
        /// </summary>
        /// <param name="runspace">The runspace.</param>
        public void PushRunspace(Runspace runspace)
        {
            this.runspaceStack.Push(runspace);
        }

        /// <summary>
        /// Pops a runspace.
        /// </summary>
        public void PopRunspace()
        {
            this.runspaceStack.Pop();
        }

        /// <inheritdoc />
        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override void NotifyBeginApplication()
        {
            // Not implemented.
        }

        /// <inheritdoc />
        public override void NotifyEndApplication()
        {
            // Not implemented.
        }

        /// <inheritdoc />
        public override void SetShouldExit(int exitCode)
        {
            this.program.ShouldExit = true;
            this.program.ExitCode = exitCode;
        }
    }
}
