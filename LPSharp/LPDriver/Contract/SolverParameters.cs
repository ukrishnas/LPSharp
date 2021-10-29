// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverParameters.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

    public class SolverParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolverParameters"/> class.
        /// </summary>
        public SolverParameters()
        {
            this.ClpParameters = new List<Param>();
            this.GlopParameters = new GlopParameters();
            this.MsfParameters = new List<Param>();
        }

        /// <summary>
        /// Gets or sets the LP algorithm. The default value is solver default.
        /// </summary>
        [DataMember]
        public LPAlgorithm LPAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the time limit in seconds. The default value is no limit.
        /// </summary>
        [DataMember]
        public long TimeLimitInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the GLOP solver specific parameters.
        /// </summary>
        [DataMember]
        public GlopParameters GlopParameters { get; set; }

        /// <summary>
        /// Gets or sets the CLP solver specific parameters.
        /// </summary>
        [DataMember]
        [XmlArray]
        [XmlArrayItem(ElementName = "Param")]
        public List<Param> ClpParameters { get; set; }

        /// <summary>
        /// Gets or sets the MSF solver specific parameters.
        /// </summary>
        [DataMember]
        [XmlArray]
        [XmlArrayItem(ElementName = "Param")]
        public List<Param> MsfParameters { get; set; }
    }
}
