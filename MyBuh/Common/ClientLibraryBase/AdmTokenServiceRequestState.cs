// -
// <copyright file="AdmTokenServiceRequestState.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii
{
    using System.Net;

    /// <summary>
    /// The request state for Adm token service.
    /// </summary>
    public class AdmTokenServiceRequestState
    {
        /// <summary>
        /// Initializes a new instance of the AdmTokenServiceRequestState class.
        /// </summary>
        public AdmTokenServiceRequestState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AdmTokenServiceRequestState class.
        /// </summary>
        /// <param name="callback">The callback delegate</param>
        /// <param name="request">The request</param>
        public AdmTokenServiceRequestState(Microsoft.Hawaii.AdmTokenService.RetriveAdmAccessTokenComplete callback, HttpWebRequest request)
        {
            this.Callback = callback;
            this.Request = request;
        }

        /// <summary>
        /// Gets or sets the callback delegates
        /// </summary>
        public Microsoft.Hawaii.AdmTokenService.RetriveAdmAccessTokenComplete Callback { get; set; }

        /// <summary>
        /// Gets or sets the http request.
        /// </summary>
        public HttpWebRequest Request { get; set; }
    }
}
