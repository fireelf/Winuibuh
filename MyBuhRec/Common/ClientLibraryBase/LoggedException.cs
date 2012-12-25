//-----------------------------------------------------------------------
// <copyright file="LoggedException.cs" company="Microsoft">
//     Copyright (c) iano, Microsoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Hawaii
{
    using System.Runtime.Serialization;

     /// <summary>
    /// Used to serialize exception information.
    /// </summary>
    [DataContract]
    public class LoggedException
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        [DataMember]
        public string StackTrace { get; set; }
    }
}
