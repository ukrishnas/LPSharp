// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Param.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a name-value pair.
    /// </summary>
    [DataContract]
    public class Param
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Param"/> class.
        /// </summary>
        public Param()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Param"/> class.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        public Param(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string Value { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.Name}={this.Value}";
        }
    }
}
