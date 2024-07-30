// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostUserInterface.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.PowershellConsole
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Security;
    using System.Text;

    /// <summary>
    /// A sample implementation of the PSHostUserInterface abstract class for console applications. Not all members are
    /// implemented. Those that are not implemented throw a NotImplementedException exception or return nothing. Members
    /// that are implemented include those that map easily to Console APIs and a basic implementation of the prompt API
    /// provided. Code originally from MSDN as a sample, adapted to fit style etc.
    /// </summary>
    internal class HostUserInterface : PSHostUserInterface
    {
        /// <summary>
        /// An instance of the PSRawUserInterface object.
        /// </summary>
        private readonly RawUserInterface myRawUi = new RawUserInterface();

        /// <summary>
        /// The host.
        /// </summary>
        private readonly PowershellHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostUserInterface"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public HostUserInterface(PowershellHost host)
        {
            this.host = host;
        }

        /// <inheritdoc />
        public override PSHostRawUserInterface RawUI => this.myRawUi;

        /// <inheritdoc />
        public override Dictionary<string, PSObject> Prompt(
            string caption,
            string message,
            Collection<FieldDescription> descriptions)
        {
            this.Write(ConsoleColor.Blue, ConsoleColor.Black, caption + "\n" + message + " ");
            var results = new Dictionary<string, PSObject>();
            foreach (FieldDescription fd in descriptions)
            {
                string[] label = GetHotkeyAndLabel(fd.Label);
                this.WriteLine(label[1]);
                string userData = Console.ReadLine();
                if (userData == null)
                {
                    return null;
                }

                results[fd.Name] = PSObject.AsPSObject(userData);
            }

            return results;
        }

        /// <inheritdoc />
        public override int PromptForChoice(
            string caption,
            string message,
            Collection<ChoiceDescription> choices,
            int defaultChoice)
        {
            // Write the caption and message strings in Blue.
            this.WriteLine(ConsoleColor.Blue, ConsoleColor.Black, caption + "\n" + message + "\n");

            // Convert the choice collection into something that is easier to
            // work with. See the BuildHotkeysAndPlainLabels method for details.
            string[,] promptData = BuildHotkeysAndPlainLabels(choices);

            // Format the overall choice prompt string to display...
            var sb = new StringBuilder();
            for (int element = 0; element < choices.Count; element++)
            {
                sb.AppendFormat(CultureInfo.CurrentCulture, "|{0}> {1} ", promptData[0, element], promptData[1, element]);
            }

            sb.AppendFormat(CultureInfo.CurrentCulture, "[Default is ({0}]", promptData[0, defaultChoice]);

            // Read prompts until a match is made, the default is
            // chosen, or the loop is interrupted with ctrl-C.
            while (true)
            {
                this.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, sb.ToString());
                string readLine = Console.ReadLine();
                if (readLine == null)
                {
                    continue;
                }

                string data = readLine.Trim().ToUpper(CultureInfo.CurrentCulture);

                // If the choice string was empty, use the default selection.
                if (data.Length == 0)
                {
                    return defaultChoice;
                }

                // See if the selection matched and return the
                // corresponding index if it did.
                for (int i = 0; i < choices.Count; i++)
                {
                    if (promptData[0, i] == data)
                    {
                        return i;
                    }
                }

                this.WriteErrorLine("Invalid choice: " + data);
            }
        }

        /// <inheritdoc />
        public override PSCredential PromptForCredential(
            string caption,
            string message,
            string userName,
            string targetName)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override PSCredential PromptForCredential(
            string caption,
            string message,
            string userName,
            string targetName,
            PSCredentialTypes allowedCredentialTypes,
            PSCredentialUIOptions options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override string ReadLine()
        {
            // call the PSConsoleHostReadline function to allow us to use PSReadline need to ship with a psm1 file that
            // provides this and use that if PSReadline isn't loaded
            using (var currentPowerShell = PowerShell.Create())
            {
                currentPowerShell.Runspace = this.host.Runspace;
                currentPowerShell.AddScript("PSConsoleHostReadline");
                return currentPowerShell.Invoke()[0].ToString();
            }
        }

        /// <inheritdoc />
        public override SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override void Write(string value)
        {
            Console.Write(value);
        }

        /// <inheritdoc />
        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            ConsoleColor oldFg = Console.ForegroundColor;
            ConsoleColor oldBg = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(value);
            Console.ForegroundColor = oldFg;
            Console.BackgroundColor = oldBg;
        }

        /// <inheritdoc />
        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            ConsoleColor oldFg = Console.ForegroundColor;
            ConsoleColor oldBg = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(value);
            Console.ForegroundColor = oldFg;
            Console.BackgroundColor = oldBg;
        }

        /// <inheritdoc />
        public override void WriteDebugLine(string message)
        {
            this.WriteLine(
                ConsoleColor.DarkYellow,
                ConsoleColor.Black,
                string.Format(CultureInfo.CurrentCulture, "DEBUG: {0}", message));
        }

        /// <inheritdoc />
        public override void WriteErrorLine(string value)
        {
            this.WriteLine(ConsoleColor.Red, ConsoleColor.Black, value);
        }

        /// <inheritdoc />
        public override void WriteLine()
        {
            Console.WriteLine();
        }

        /// <inheritdoc />
        public override void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        /// <inheritdoc />
        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
        }

        /// <inheritdoc />
        public override void WriteVerboseLine(string message)
        {
            this.WriteLine(
                ConsoleColor.Green,
                ConsoleColor.Black,
                string.Format(CultureInfo.CurrentCulture, "VERBOSE: {0}", message));
        }

        /// <inheritdoc />
        public override void WriteWarningLine(string message)
        {
            this.WriteLine(
                ConsoleColor.Yellow,
                ConsoleColor.Black,
                string.Format(CultureInfo.CurrentCulture, "WARNING: {0}", message));
        }

        /// <summary>
        /// This is a private worker function splits out the accelerator keys from the menu and builds a two dimensional
        /// array with the first access containing the accelerator and the second containing the label string with the
        /// &amp; removed.
        /// </summary>
        /// <param name="choices">The choice collection to process.</param>
        /// <returns>A two dimensional array containing the accelerator characters and the cleaned-up labels.</returns>
        private static string[,] BuildHotkeysAndPlainLabels(
            IReadOnlyList<ChoiceDescription> choices)
        {
            // Allocate the result array.
            var hotkeysAndPlainLabels = new string[2, choices.Count];
            for (int i = 0; i < choices.Count; ++i)
            {
                string[] hotkeyAndLabel = GetHotkeyAndLabel(choices[i].Label);
                hotkeysAndPlainLabels[0, i] = hotkeyAndLabel[0];
                hotkeysAndPlainLabels[1, i] = hotkeyAndLabel[1];
            }

            return hotkeysAndPlainLabels;
        }

        /// <summary>
        /// Parses a string containing a hotkey character.
        /// Take a string of the form
        ///    Yes to &amp;all
        /// and returns a two-dimensional array split out as
        ///    "A", "Yes to all".
        /// </summary>
        /// <param name="input">The string to process.</param>
        /// <returns>A two dimensional array containing the parsed components.</returns>
        private static string[] GetHotkeyAndLabel(string input)
        {
            string[] result = { string.Empty, string.Empty };
            string[] fragments = input.Split('&');
            if (fragments.Length == 2)
            {
                if (fragments[1].Length > 0)
                {
                    result[0] = fragments[1][0].ToString().ToUpper(CultureInfo.CurrentCulture);
                }

                result[1] = (fragments[0] + fragments[1]).Trim();
            }
            else
            {
                result[1] = input;
            }

            return result;
        }
    }
}