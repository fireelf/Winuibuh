// -
// <copyright file="AdmAccessToken.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The adm access token.
    /// </summary>
    [DataContract]
    public class AdmAccessToken
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the seconds token will expire in.
        /// </summary>
        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the time token will expire.
        /// </summary>
        public DateTime ExpriesAtUtc { get; set; }
    }
}
