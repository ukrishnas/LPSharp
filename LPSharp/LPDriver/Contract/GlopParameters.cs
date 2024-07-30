// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlopParameters.cs">
// Copyright (c) Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents common and solver-specific parameters.
    /// </summary>
    [DataContract]

    public class GlopParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlopParameters"/> class.
        /// </summary>
        public GlopParameters()
        {
            this.Parameters = new List<Param>();
        }

        /// <summary>
        /// Gets or sets the common solver parameters.
        /// </summary>
        [DataMember]
        [XmlArray]
        [XmlArrayItem(ElementName = "Param")]
        public List<Param> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the underlying solver specific parameters as protocol buffer text.
        /// </summary>
        [DataMember]
        public string SolverSpecificParameterText { get; set; }
    }
}
