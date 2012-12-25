//-----------------------------------------------------------------------
// <copyright file="ServiceFault.cs" company="Microsoft">
//     Copyright (c) iano, Microsoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Hawaii
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an error returned to the client.
    /// </summary>
    [DataContract]
    public class ServiceFault
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the correlation ID of the message.
        /// </summary>
        /// <value>
        /// The id uniquely identifying the message
        /// </value>
        [DataMember]
        public Guid RequestId { get; set; }

        /// <summary>
        /// Gets or sets the exception stack.
        /// </summary>
        /// <value>
        /// The exception stack.
        /// </value>
        [DataMember]
        public IEnumerable<LoggedException> ExceptionStack { get; set; }
    }
}
