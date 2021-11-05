﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents utility methods.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// The default XML writer settings are to write elements in new lines and indent them.
        /// </summary>
        private static XmlWriterSettings defaultXmlWriterSettings = new XmlWriterSettings { Indent = true };

        /// <summary>
        /// Sets class public properties from list of parameters.
        /// </summary>
        /// <param name="parameters">The list of parameters.</param>
        /// <param name="obj">The instance of class.</param>
        /// <typeparam name="T">The class type.</typeparam>
        public static void SetPropertiesFromList<T>(IList<Param> parameters, T obj)
            where T : class
        {
            if (parameters == null)
            {
                return;
            }

            // Gets the instance public properties (non-indexers and with setter) for type T.
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                        .Where(pi => pi.CanWrite && pi.GetIndexParameters().Length == 0)
                                        .ToDictionary(pi => pi.Name);

            foreach (var parameter in parameters)
            {
                if (!properties.TryGetValue(parameter.Name, out PropertyInfo property))
                {
                    continue;
                }

                var converter = TypeDescriptor.GetConverter(property.PropertyType);
                if (!converter.CanConvertFrom(typeof(string)))
                {
                    continue;
                }

                var value = converter.ConvertFrom(parameter.Value);
                property.SetValue(obj, value, null);
            }
        }

        /// <summary>
        /// De-serializes a stream into object using data contract or XML serialization.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="obj">The created object.</param>
        /// <returns>True on success, false otherwise.</returns>
        /// <typeparam name="T">The type of the object.</typeparam>
        public static bool TryDeserialize<T>(Stream stream, out T obj)
            where T : class
        {
            obj = default;

            using (var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max))
            {
                try
                {
                    var xmlDeserializer = new XmlSerializer(typeof(T));
                    obj = (T)xmlDeserializer.Deserialize(reader);
                }
                catch (InvalidOperationException)
                {
                    try
                    {
                        var contractSerializer = new DataContractSerializer(typeof(T));
                        obj = (T)contractSerializer.ReadObject(reader);
                    }
                    catch (SerializationException)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Serializes an object to a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="settings">The XML writer settings.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>True on success, false otherwise.</returns>
        public static bool TrySerialize<T>(Stream stream, T obj, XmlWriterSettings settings = null)
            where T : class
        {
            if (settings == null)
            {
                settings = defaultXmlWriterSettings;
            }

            using (var writer = XmlWriter.Create(stream, settings))
            {
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(writer, obj);
                }
                catch (InvalidOperationException)
                {
                    try
                    {
                        var contractSerializer = new DataContractSerializer(typeof(T));
                        contractSerializer.WriteObject(writer, obj);
                    }
                    catch (SerializationException)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}