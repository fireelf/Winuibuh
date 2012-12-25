// -
// <copyright file="GuidAuthClientIdentity.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii
{
    using System;
    using System.Text;

    /// <summary>
    /// The Hawaii Guid authentication client identity.
    /// </summary>
    public class GuidAuthClientIdentity : ClientIdentity
    {
        /// <summary>
        /// Initializes a new instance of the GuidAuthClientIdentity class.
        /// </summary>
        /// <param name="applicationId">The hawaii application Id.</param>
        public GuidAuthClientIdentity(string applicationId) :
            this(applicationId, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GuidAuthClientIdentity class.
        /// </summary>
        /// <param name="applicationId">The hawaii application Id.</param>
        /// <param name="registrationId">The registration Id.</param>
        /// <param name="secretKey">The secret key.</param>
        public GuidAuthClientIdentity(string applicationId, string registrationId, string secretKey) :
            base(registrationId, secretKey)
        {
            this.ApplicationId = applicationId;
        }

        /// <summary>
        /// Gets the Hawaii application Id.
        /// </summary>
        public string ApplicationId { get; private set; }

        /// <summary>
        /// Override the method to copy Guid authentication identity
        /// </summary>
        /// <returns>Returns the client identity</returns>
        public override ClientIdentity Copy()
        {
            return new GuidAuthClientIdentity(this.ApplicationId, this.RegistrationId, this.SecretKey);
        }

        /// <summary>
        /// Override the method to retrive the access token for hawaii Guid authentication.
        /// </summary>
        /// <param name="callback">callback from event</param>
        public override void RetriveAccessToken(RetriveAccessTokenComplete callback)
        {
            string token = string.Empty;

            if (!string.IsNullOrEmpty(this.RegistrationId) &&
                !string.IsNullOrEmpty(this.SecretKey))
            {
                if (!string.IsNullOrEmpty(this.ApplicationId))
                {
                    // If this app token is not empty, take only id and secret key
                    token = string.Format("{0}:{1}:{2}", this.ApplicationId, this.RegistrationId, this.SecretKey);
                }
                else
                {
                    token = string.Format("{0}:{1}", this.RegistrationId, this.SecretKey);
                }
            }
            else if (!string.IsNullOrEmpty(this.ApplicationId))
            {
                token = this.ApplicationId;
            }

            if (!string.IsNullOrEmpty(token))
            {
                this.OnRetriveAccessTokenComplete(string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(token))), null, callback);
            }
            else
            {
                this.OnRetriveAccessTokenComplete(string.Empty, new InvalidOperationException("Hawaii Guid authentication token cannot be empty string"), callback);
            }
        }
    }
}
