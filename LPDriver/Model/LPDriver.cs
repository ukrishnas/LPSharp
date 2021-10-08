// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPEngine.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the driver for LPSharp. It holds common state and methods.
    /// </summary>
    public class LPDriver
    {
        /// <summary>
        /// The collection of models.
        /// </summary>
        private readonly Dictionary<string, LPModel> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPDriver"/> class.
        /// </summary>
        public LPDriver()
        {
            this.models = new Dictionary<string, LPModel>();
            this.MpsReader = new MpsReader();
        }

        /// <summary>
        /// Gets the enumeration of models.
        /// </summary>
        public IReadOnlyDictionary<string, LPModel> Models => this.models;

        /// <summary>
        /// Gets the MPS file reader.
        /// </summary>
        public MpsReader MpsReader { get; }

        /// <summary>
        /// Adds or updates a model.
        /// </summary>
        /// <param name="key">The model key.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool AddModel(string key, LPModel model)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            // Previous model is silently overwritten.
            this.models[key] = model;
            return true;
        }

        /// <summary>
        /// Gets a model by key.
        /// </summary>
        /// <param name="key">The mode name.</param>
        /// <returns>The model.</returns>
        public LPModel GetModel(string key)
        {
            this.models.TryGetValue(key, out LPModel model);
            return model;
        }

        /// <summary>
        /// Clears stored models.
        /// </summary>
        public void ClearModels()
        {
            this.models.Clear();
        }
    }
}