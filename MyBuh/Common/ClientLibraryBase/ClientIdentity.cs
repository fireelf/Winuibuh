// -
// <copyright file="ClientIdentity.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii
{
    using System;
    using System.Text;

    /// <summary>
    /// ClientIdentity represents a client identity for the purposes of communicating with the server.
    /// </summary>
    public abstract class ClientIdentity
    {
        /// <summary>
        /// Initializes a new instance of the ClientIdentity class.
        /// </summary>
        public ClientIdentity() :
            this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClientIdentity class.
        /// </summary>
        /// <param name="registrationId">Specifies a registration id.</param>
        /// <param name="secretKey">Specifies a secret key.</param>
        public ClientIdentity(string registrationId, string secretKey)
        {
            this.RegistrationId = registrationId;
            this.SecretKey = secretKey;
        }

        /// <summary>
        /// The delegate of RetriveAccessTokenEvent event.
        /// </summary>
        /// <param name="accessToken">The access token string.</param>
        /// <param name="ex">Coressponding exception if failed to get the access token.</param>
        public delegate void RetriveAccessTokenComplete(string accessToken, Exception ex);

        /// <summary>
        /// Gets or sets the registration id.
        /// </summary>
        public string RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the secret key.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets the identity token that is used when communicating with the server.
        /// </summary>
        /// <param name="callback">callback from event</param>
        public abstract void RetriveAccessToken(RetriveAccessTokenComplete callback);

        /// <summary>
        /// Derived class provides implementation.
        /// </summary>
        /// <returns>Returns the client identity</returns>
        public abstract ClientIdentity Copy();

        /// <summary>
        /// Helper method to fire the RetriveAccessTokenCompleteEvent event.
        /// </summary>
        /// <param name="accessToken">The accesss token string</param>
        /// <param name="ex">Coressponding exception if failed to get the access token.</param>
        /// <param name="callback">callback from event</param>
        protected virtual void OnRetriveAccessTokenComplete(string accessToken, Exception ex, RetriveAccessTokenComplete callback)
        {
            callback(accessToken, ex);
        }
    }
}
