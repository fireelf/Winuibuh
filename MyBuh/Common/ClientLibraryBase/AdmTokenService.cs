// -
// <copyright file="AdmTokenService.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// The adm token service.
    /// </summary>
    public class AdmTokenService
    {
        /// <summary>
        /// The token expired in minutes.
        /// </summary>
        private static readonly int TokenExpiredInMinutes = 40;

        /// <summary>
        /// The latency from local to Hawaii service in second.
        /// </summary>
        private static readonly int NetworkLatency = 5;

        /// <summary>
        /// The static sync root object for lock.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The instance of Adm Access Token.
        /// </summary>
        private AdmAccessToken accessToken = null;

        /// <summary>
        /// The Azure DataMarket token service access Uri.
        /// </summary>
        private string dataMarketAccessUri = string.Empty;

        /// <summary>
        /// The client Id to access Hawaii Service.
        /// </summary>
        private string clientId = string.Empty;

        /// <summary>
        /// The client Secret to access Hawaii Service.
        /// </summary>
        private string clientSecret = string.Empty;

        /// <summary>
        /// The Hawaii service scope.
        /// </summary>
        private string targetServiceScope = string.Empty;

        /// <summary>
        /// The request details to retrive Adm access token.
        /// </summary>
        private string requestDetails = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdmTokenService"/> class.
        /// </summary>
        /// <param name="clientId">The Adm client Id.</param>
        /// <param name="clientSecret">The Adm client secret.</param>
        /// <param name="dataMarketAccessUri">The Adm OAuth endpoint.</param>
        /// <param name="targetServiceScope">The target service scope.</param>
        public AdmTokenService(string clientId, string clientSecret, string dataMarketAccessUri, string targetServiceScope)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.dataMarketAccessUri = dataMarketAccessUri;
            this.targetServiceScope = targetServiceScope;

            this.requestDetails = string.Format(
                "grant_type=client_credentials&client_id={0}&client_secret={1}&scope={2}",
                Uri.EscapeDataString(this.clientId),
                Uri.EscapeDataString(this.clientSecret),
                Uri.EscapeUriString(this.targetServiceScope));
        }

        /// <summary>
        /// The delegate of GetAdmAccessToken event.
        /// </summary>
        /// <param name="accessToken">The adm access token object.</param>
        /// <param name="ex">Coressponding exception if failed to get adm access token.</param>
        public delegate void RetriveAdmAccessTokenComplete(AdmAccessToken accessToken, Exception ex);

        /// <summary>
        /// Gets a value indicating whether need to renew the Adm access token.
        /// </summary>
        private bool NeedToReNew
        {
            get
            {
                return this.accessToken == null || this.accessToken.ExpriesAtUtc < DateTime.UtcNow.AddSeconds(NetworkLatency);
            }
        }

        /// <summary>
        /// Gets the Adm access token string.
        /// </summary>
        /// <param name="callback">callback for the completion of request</param>
        public void GetAccessToken(RetriveAdmAccessTokenComplete callback)
        {
            if (this.NeedToReNew)
            {
                lock (SyncRoot)
                {
                    if (this.NeedToReNew)
                    {
                        this.RetrieveAccessToken(callback);
                    }
                    else
                    {
                        this.OnRetriveAdmAccessTokenComplete(this.accessToken, null, callback);
                    }
                }
            }
            else
            {
                this.OnRetriveAdmAccessTokenComplete(this.accessToken, null, callback);
            }
        }

        /// <summary>
        /// Help method to fire the RetriveAccessTokenCompleteEvent event.
        /// </summary>
        /// <param name="accessToken">The accesss token string</param>
        /// <param name="ex">Coressponding exception if failed to get the access token.</param>
        /// <param name="callback">callback from event</param>
        protected virtual void OnRetriveAdmAccessTokenComplete(AdmAccessToken accessToken, Exception ex, RetriveAdmAccessTokenComplete callback)
        {
            callback(accessToken, ex);
        }

        /// <summary>
        /// Retrive new access token from Adm service.
        /// </summary>
        /// <param name="callback">callback from event</param>
        private void RetrieveAccessToken(RetriveAdmAccessTokenComplete callback)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.dataMarketAccessUri);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";

                request.BeginGetRequestStream(new AsyncCallback(this.RequestCallback), new AdmTokenServiceRequestState(callback, request));
            }
            catch (Exception ex)
            {
                this.OnRetriveAdmAccessTokenComplete(null, ex, callback);
            }
        }

        /// <summary>
        /// Callback method called after request.BeginGetRequestStream completes.
        /// </summary>
        /// <param name="asyncResult">
        /// An asyncResult object.
        /// </param>
        private void RequestCallback(IAsyncResult asyncResult)
        {
            AdmTokenServiceRequestState stateObject = (AdmTokenServiceRequestState)asyncResult.AsyncState;
            try
            {
                HttpWebRequest request = stateObject.Request;
                using (Stream stream = request.EndGetRequestStream(asyncResult))
                {
                    if (stream == null)
                    {
                        throw new Exception("Null/Invalid request stream received from server.");
                    }

                    byte[] inputBuffer = Encoding.UTF8.GetBytes(this.requestDetails);

                    if (inputBuffer != null && inputBuffer.Length != 0)
                    {
                        stream.Write(inputBuffer, 0, inputBuffer.Length);
                    }
                }

                request.BeginGetResponse(new AsyncCallback(this.ResponseCallback), stateObject);
            }
            catch (Exception ex)
            {
                this.OnRetriveAdmAccessTokenComplete(null, ex, stateObject.Callback);
            }
        }

        /// <summary>
        /// Callback method called after request.BeginGetResponse completes.
        /// </summary>
        /// <param name="asyncResult">
        /// An asyncResult object.
        /// </param>
        private void ResponseCallback(IAsyncResult asyncResult)
        {
            AdmTokenServiceRequestState stateObject = (AdmTokenServiceRequestState)asyncResult.AsyncState;
            try
            {
                HttpWebRequest state = stateObject.Request;
                using (HttpWebResponse response = (HttpWebResponse)state.EndGetResponse(asyncResult))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));

                    this.accessToken = (AdmAccessToken)serializer.ReadObject(response.GetResponseStream());
                    this.accessToken.ExpriesAtUtc = DateTime.UtcNow.AddMinutes(TokenExpiredInMinutes);

                    this.OnRetriveAdmAccessTokenComplete(this.accessToken, null, stateObject.Callback);
                }
            }
            catch (Exception ex)
            {
                this.OnRetriveAdmAccessTokenComplete(null, ex, stateObject.Callback);
            }
        }
    }
}
