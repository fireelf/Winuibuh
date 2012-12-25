// -
// <copyright file="ServiceAgent.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Xml.Serialization;

    /// <summary>
    /// A base class for all Hawaii service agent classes. 
    /// These agents are wrapping the communication tasks specific to each service.
    /// ServiceAgent provides functionality common to all these clases.
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    public abstract class ServiceAgent<T> where T : ServiceResult, new()
    {
        /// <summary>
        /// The http request object.
        /// </summary>
        private HttpWebRequest request;

        /// <summary>
        /// Initializes a new instance of the ServiceAgent class.
        /// </summary>
        public ServiceAgent() :
            this(HttpMethod.Get, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServiceAgent class.
        /// </summary>
        /// <param name="requestMethod">Specifies a http request method.</param>
        /// <param name="stateObject">Specifies a user-defined object.</param> 
        public ServiceAgent(HttpMethod requestMethod, object stateObject)
        {
            this.RequestMethod = requestMethod;
            this.Uri = null;
            this.ClientIdentity = null;
            this.Result = new T();
            this.Result.StateObject = stateObject;
        }

        /// <summary>
        /// OnCompleteDelegate delegate type definition.
        /// </summary>
        /// <param name="result">
        /// Returns nothing.
        /// </param>
        public delegate void OnCompleteDelegate(T result);

        /// <summary>
        /// Gets or sets OnComplete handler.
        /// </summary>
        protected OnCompleteDelegate OnComplete { get; set; }

        /// <summary>
        /// Gets or sets service result.
        /// </summary>
        protected T Result { get; set; }

        /// <summary>
        /// Gets or sets service Uri.
        /// </summary>
        protected Uri Uri { get; set; }

        /// <summary>
        /// Gets the request content type. 
        /// </summary>
        /// <remarks>
        /// Default is application/xml for backwards compatibility.
        /// </remarks>
        protected virtual string RequestContentType
        {
            get
            {
                return "application/xml";
            }
        }

        /// <summary>
        /// Gets or sets the HTTP method (GET, POST, PUT or DELETE).
        /// </summary>
        protected HttpMethod RequestMethod { get; set; }

        /// <summary>
        /// Gets or sets the client identity.
        /// </summary>
        protected ClientIdentity ClientIdentity { get; set; }

        /// <summary>
        /// This method initiates the asynchronous service call.
        /// </summary>
        /// <param name="handlerMethod">
        /// The on complete" callback that will be invoked after the service call completes.
        /// </param>
        public void ProcessRequest(OnCompleteDelegate handlerMethod)
        {
            try
            {
                if (handlerMethod != null)
                {
                    this.OnComplete = handlerMethod;
                }

                // Create the Http request.
                this.request = (HttpWebRequest)HttpWebRequest.Create(this.Uri);

                // Set http method.
                this.request.Method = this.RequestMethod.ToString().ToUpper();

                // Set expected format of response
                this.request.Accept = this.RequestContentType;

                if (this.ClientIdentity != null)
                {
                    this.ClientIdentity.RetriveAccessToken(new Hawaii.ClientIdentity.RetriveAccessTokenComplete(this.ClientIdentity_RetriveAccessTokenCompleteEvent));
                }
                else
                {
                    this.SendHttpRequest(string.Empty);
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Deserializes the response stream.
        /// </summary>
        /// <typeparam name="TResult">Result object.</typeparam>
        /// <param name="responseStream">Server response stream.</param>
        /// <returns>Deserailized object.</returns>
        protected static TResult DeserializeResponse<TResult>(Stream responseStream) where TResult : class
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TResult));
                return serializer.Deserialize(responseStream) as TResult;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Invalid response received from server.", ex);
            }
        }

        /// <summary>
        /// Deserializes the response stream using JSON serializer.
        /// </summary>
        /// <typeparam name="TResult">Result object.</typeparam>
        /// <param name="responseStream">Server response stream.</param>
        /// <returns>Deserailized object.</returns>
        protected static TResult DeserializeResponseJson<TResult>(Stream responseStream) where TResult : class
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TResult));
                TResult result = serializer.ReadObject(responseStream) as TResult;
                return result;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Invalid response received from server.", ex);
            }
        }

        /// <summary>
        /// This method must be implemented by all classes that inherit from ServiceAgent. 
        /// It will provide the POST data that has to be sent to the service if the Http Method used is POST.
        /// </summary>
        /// <returns>Returns the POST data in byte array format.</returns>
        protected virtual byte[] GetPostData()
        {
            return null;
        }

        /// <summary>
        /// This method is called after the response sent by the server is received by the client.
        /// It allows classes that inherit from ServiceAgent to do their own processing of 
        /// the data received from the server.
        /// </summary>
        /// <param name="responseStream">
        /// The response stream containing response data that is received from the server.
        /// </param>
        protected virtual void ParseOutput(Stream responseStream)
        {
        }

        /// <summary>
        /// A virtual method. It is invoked after completing the service request.
        /// The implementation of this base class will invoke the client's "on complete" callback method.
        /// Classes that inherit from ServiceAgent can oveerite this method to further process the service 
        /// call result before calling the client's "on complete" callback method.
        /// </summary>
        protected virtual void OnCompleteRequest()
        {
            if (this.OnComplete != null)
            {
                // Call the UI's on completion method.
                this.OnComplete(this.Result);
            }
        }

        /// <summary>
        /// The callback handler of ClientIdentity after get the access token.
        /// </summary>
        /// <param name="accessToken">The authorization token.</param>
        /// <param name="ex">Coressponding exception if failed to get the access token.</param>
        private void ClientIdentity_RetriveAccessTokenCompleteEvent(string accessToken, Exception ex)
        {  
            if (ex == null)
            {
                this.SendHttpRequest(accessToken);
            }
            else
            {
                this.HandleException(ex);
            }
        }

        /// <summary>
        /// Send the http request.
        /// </summary>
        /// <param name="identityToken">The authorization token.</param>
        private void SendHttpRequest(string identityToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(identityToken))
                {
                    // if the identity token is not null, set the authorization header.
                    this.request.Headers[HttpRequestHeader.Authorization] = identityToken;
                }

                if (this.RequestMethod != HttpMethod.Get)
                {
                    // Set the content body type of the request
                    this.request.ContentType = this.RequestContentType;

                    this.request.BeginGetRequestStream(new AsyncCallback(this.RequestCallback), this.request);
                }
                else
                {
                    this.request.BeginGetResponse(new AsyncCallback(this.ResponseCallback), this.request);
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
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
            Debug.Assert(asyncResult != null, "IAsyncResult object is null");
            Debug.Assert(asyncResult.AsyncState != null, "IAsyncResult.AsyncState object is null");

            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            Debug.Assert(request != null, "HttpWebRequest object is null");

            try
            {
                using (Stream stream = request.EndGetRequestStream(asyncResult))
                {
                    if (stream == null)
                    {
                        throw new Exception("Null/Invalid request stream received from server.");
                    }

                    // Get the input from the service client.
                    byte[] inputBuffer = this.GetPostData();

                    // Step 3: POST data, for a POST request.
                    if (inputBuffer != null && inputBuffer.Length != 0)
                    {
                        stream.Write(inputBuffer, 0, inputBuffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            request.BeginGetResponse(new AsyncCallback(this.ResponseCallback), request);
        }

        /// <summary>
        /// Callback method called after request.BeginGetResponse completes.
        /// </summary>
        /// <param name="asyncResult">
        /// An asyncResult object.
        /// </param>
        private void ResponseCallback(IAsyncResult asyncResult)
        {
            Debug.Assert(asyncResult != null, "IAsyncResult object is null");
            Debug.Assert(asyncResult.AsyncState != null, "IAsyncResult.AsyncState object is null");

            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            Debug.Assert(request != null, "HttpWebRequest object is null");

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult))
                {
                    if (response == null)
                    {
                        // Create and set the error exception.
                        throw new Exception("Invalid response from server.");
                    }

                    this.Result.Status = this.ConvertStatus(response.StatusCode);
                    if (this.Result.Status != Status.Success)
                    {
                        // Create and set the error exception.
                        throw new Exception("Invalid result status from server.");
                    }

                    // Calls client method to interprete the result buffer.
                    this.ParseOutput(response.GetResponseStream());

                    // The following code will be never hit, but kept it for safety.
                    if (this.Result.Exception != null &&
                        this.Result.Status == Status.Success)
                    {
                        // This is be an invalid combination.
                        Debug.Assert(false, "Result status can't be success when exception indicates an error.");
                        throw new Exception("Invalid response stream received from server.");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Result.Status = Status.Error;
                System.Net.WebException webException = ex as System.Net.WebException;
                if (webException == null || webException.Response == null)
                {
                    this.Result.Exception = ex;
                }
                else
                {
                    using (Stream stream = webException.Response.GetResponseStream())
                    {
                        try
                        {
                            ServiceFault fault = DeserializeResponseJson<ServiceFault>(stream);

                            if (fault == null)
                            {
                                this.Result.Exception = ex;
                            }
                            else
                            {
                                this.Result.Exception = new WebException(fault.Message, ex);
                                this.Result.RequestId = fault.RequestId;
                                this.Result.ServerExceptionStack = fault.ExceptionStack;
                            }
                        }
                        catch (SerializationException)
                        {
                            this.Result.Exception = ex;
                        }
                    }
                }
            }
            finally
            {
                // Calls client method to call service event hanlder.
                this.OnCompleteRequest();
            }
        }

        /// <summary>
        /// Handle unexpected exception and fires the UI callback.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        private void HandleException(Exception ex)
        {
            this.Result.Status = Status.Error;
            this.Result.Exception = ex;

            // Call the OnCompleteRequest handler in case any error handling request.
            this.OnCompleteRequest();
        }

        /// <summary>
        /// Method converts HttpStatusCode to Status.
        /// </summary>
        /// <param name="statusCode">Service http status code.</param>
        /// <returns>Hawaii Status code.</returns>
        private Status ConvertStatus(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    return Status.Success;

                case HttpStatusCode.InternalServerError:
                    return Status.InternalServerError;

                default:
                    return Status.Error;
            }
        }
    }
}
