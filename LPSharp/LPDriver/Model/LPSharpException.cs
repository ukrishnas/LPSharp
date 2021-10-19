﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPSharpException.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;

    /// <summary>
    /// Represents a generic exception raised by modules in this project.
    /// </summary>
    [Serializable]
    public class LPSharpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LPSharpException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public LPSharpException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LPSharpException"/> class.
        /// </summary>
        /// <param name="format">The exception message format.</param>
        /// <param name="objects">The exception message objects.</param>
        public LPSharpException(string format, params object[] objects)
            : base(string.Format(format, objects))
        {
        }
    }
}
