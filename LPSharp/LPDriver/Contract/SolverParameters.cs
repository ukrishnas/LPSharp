// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverParameters.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriver.Contract
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
            this.GenericParameters = new List<Param>();
            this.ClpParameters = new List<Param>();
            this.GlopParameters = new GlopParameters();
            this.MsfParameters = new List<Param>();
        }

        /// <summary>
        /// Gets or sets the generic parameters that are common to all solvers. For example,
        /// LP algorithm, time limit, and enable detailed logging.
        /// </summary>
        [DataMember]
        [XmlArrayItem(ElementName = "Param")]
        public List<Param> GenericParameters { get; set; }

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
