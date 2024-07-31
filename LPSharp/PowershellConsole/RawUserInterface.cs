// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RawUserInterface.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.PowershellConsole
{
    using System;
    using System.Management.Automation.Host;

    /// <summary>
    /// The raw user interface. Code originally from MSDN as a sample, adapted to fit style etc.
    /// </summary>
    public class RawUserInterface : PSHostRawUserInterface
    {
        /// <inheritdoc />
        public override ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        /// <inheritdoc />
        public override Size BufferSize
        {
            get => new Size(Console.BufferWidth, Console.BufferHeight);
            set => Console.SetBufferSize(value.Width, value.Height);
        }

        /// <inheritdoc />
        public override Coordinates CursorPosition
        {
            get => new Coordinates(Console.CursorLeft, Console.CursorTop);
            set
            {
                Console.CursorLeft = value.X;
                Console.CursorTop = value.Y;
            }
        }

        /// <inheritdoc />
        public override int CursorSize
        {
            get => Console.CursorSize;
            set => Console.CursorSize = value;
        }

        /// <inheritdoc />
        public override ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        /// <inheritdoc />
        public override bool KeyAvailable
        {
            get => Console.KeyAvailable;
        }

        /// <inheritdoc />
        public override Size MaxPhysicalWindowSize
        {
            get => new Size(Console.LargestWindowWidth, Console.LargestWindowHeight);
        }

        /// <inheritdoc/>
        public override Size MaxWindowSize
        {
            get => new Size(Console.LargestWindowWidth, Console.LargestWindowHeight);
        }

        /// <inheritdoc />
        public override Coordinates WindowPosition
        {
            get => new Coordinates(Console.WindowLeft, Console.WindowTop);
            set => Console.SetWindowPosition(value.X, value.Y);
        }

        /// <inheritdoc />
        public override Size WindowSize
        {
            get => new Size(Console.WindowWidth, Console.WindowHeight);
            set => Console.SetWindowSize(value.Width, value.Height);
        }

        /// <inheritdoc />
        public override string WindowTitle
        {
            get => Console.Title;
            set => Console.Title = value;
        }

        /// <inheritdoc />
        public override void FlushInputBuffer()
        {
        }

        /// <inheritdoc />
        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
