// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPSolverAbstract.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;

    /// <summary>
    /// Represents an abstract class for an LP solver inside LPSharp.
    /// </summary>
    public abstract class LPSolverAbstract
    {
        /// <summary>
        /// The default lower and upper bounds for variables.
        /// </summary>
        private Tuple<double, double> defaultVariableBounds;

        /// <summary>
        /// The default lower and upper bounds for constraints.
        /// </summary>
        private Tuple<double, double> defaultConstraintBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPSolverAbstract"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        public LPSolverAbstract(string key)
        {
            this.Key = key;

            this.DefaultVariableBounds = new(0, double.PositiveInfinity);
            this.DefaultConstraintBounds = new(double.NegativeInfinity, double.PositiveInfinity);
        }

        /// <summary>
        /// Gets or sets the solver key.
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Gets or sets the default lower and upper bound for variables.
        /// </summary>
        public Tuple<double, double> DefaultVariableBounds
        {
            get => this.defaultVariableBounds;
            set
            {
                if (value.Item1 <= value.Item2)
                {
                    this.defaultVariableBounds = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the default lower and upper bound for constraints.
        /// </summary>
        public Tuple<double, double> DefaultConstraintBounds
        {
            get => this.defaultConstraintBounds;
            set
            {
                if (value.Item1 <= value.Item2)
                {
                    this.defaultConstraintBounds = value;
                }
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"key={this.Key}";
        }
    }
}
