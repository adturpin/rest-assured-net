﻿// <copyright file="RequestSpecification.cs" company="On Test Automation">
// Copyright 2019 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using RestAssuredNet.RA.Internal;
using Stubble.Core;
using Stubble.Core.Builders;

namespace RestAssuredNet.RA
{
    /// <summary>
    /// The request to be sent.
    /// </summary>
    public class RequestSpecification : IDisposable
    {
        private HttpRequestMessage request = new HttpRequestMessage();
        private object requestBody = string.Empty;
        private string contentTypeHeader = "application/json";
        private Encoding contentEncoding = Encoding.UTF8;
        private Dictionary<string, string> queryParams = new Dictionary<string, string>();
        private Dictionary<string, string> pathParams = new Dictionary<string, string>();
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSpecification"/> class.
        /// </summary>
        public RequestSpecification()
        {
        }

        /// <summary>
        /// Adds a request header and the associated value to the request object to be sent.
        /// </summary>
        /// <param name="key">The header key that is to be added to the request.</param>
        /// <param name="value">The associated header value that is to be added to the request.</param>
        /// <returns>The current <see cref="RequestSpecification"/>.</returns>
        public RequestSpecification Header(string key, object value)
        {
            this.request.Headers.Add(key, value.ToString());
            return this;
        }

        /// <summary>
        /// Add a request header and the associated values to the request object to be sent.
        /// </summary>
        /// <param name="key">The header key that is to be added to the request.</param>
        /// <param name="values">The associated header values that are to be added to the request.</param>
        /// <returns>The current <see cref="RequestSpecification"/>.</returns>
        public RequestSpecification Header(string key, IEnumerable<string> values)
        {
            this.request.Headers.Add(key, values);
            return this;
        }

        /// <summary>
        /// Add a Content-Type header and the specified value to the request object to be sent.
        /// </summary>
        /// <param name="contentType">The value for the Content-Type header to be added.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification ContentType(string contentType)
        {
            this.contentTypeHeader = contentType;
            return this;
        }

        /// <summary>
        /// Set the content character encoding for the request object to be sent.
        /// </summary>
        /// <param name="encoding">The value for the character encoding to be added.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification ContentEncoding(Encoding encoding)
        {
            this.contentEncoding = encoding;
            return this;
        }

        /// <summary>
        /// Set the value for the Accept header for the request object to be sent.
        /// </summary>
        /// <param name="accept">The value for the Accept header to be added.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification Accept(string accept)
        {
            this.request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
            return this;
        }

        /// <summary>
        /// Add a query parameter to the endpoint when the request is sent.
        /// </summary>
        /// <param name="key">The query parameter name.</param>
        /// <param name="value">The associated query parameter value.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification QueryParam(string key, object value)
        {
            this.queryParams[key] = value.ToString();
            return this;
        }

        /// <summary>
        /// Adds the specified query parameters to the endpoint when the request is sent.
        /// </summary>
        /// <param name="queryParams">A <see cref="Dictionary{TKey, TValue}"/> containing the query parameters to be added.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification QueryParams(Dictionary<string, object> queryParams)
        {
            queryParams.ToList().ForEach(param => this.queryParams[param.Key] = param.Value.ToString());
            return this;
        }

        /// <summary>
        /// Add a path parameter to the endpoint when the request is sent.
        /// </summary>
        /// <param name="key">The path parameter name.</param>
        /// <param name="value">The associated path parameter value.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification PathParam(string key, object value)
        {
            this.pathParams[key] = value.ToString();
            return this;
        }

        /// <summary>
        /// Adds the specified path parameters to the endpoint when the request is sent.
        /// </summary>
        /// <param name="pathParams">A <see cref="Dictionary{TKey, TValue}"/> containing the path parameters to be added.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification PathParams(Dictionary<string, object> pathParams)
        {
            pathParams.ToList().ForEach(param => this.pathParams[param.Key] = param.Value.ToString());
            return this;
        }

        /// <summary>
        /// Adds a basic authorization header to the request.
        /// </summary>
        /// <param name="username">The username to be used for authorization.</param>
        /// <param name="password">The password to be used for authorization.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification BasicAuth(string username, string password)
        {
            string base64EncodedBasicAuthDetails = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            this.request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedBasicAuthDetails);
            return this;
        }

        /// <summary>
        /// Adds an OAuth2 authorization token to the request.
        /// </summary>
        /// <param name="token">The OAuth2 token to be added to the request.</param>
        /// <returns>The current <see cref="RequestSpecification"/> object.</returns>
        public RequestSpecification OAuth2(string token)
        {
            this.request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }

        /// <summary>
        /// Adds a request body to the request object to be sent.
        /// </summary>
        /// <param name="body">The body that is to be sent with the request.</param>
        /// <returns>The current <see cref="RequestSpecification"/>.</returns>
        public RequestSpecification Body(object body)
        {
            this.requestBody = body;
            return this;
        }

        /// <summary>
        /// Syntactic sugar that makes tests read more like natural language.
        /// </summary>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public RequestSpecification And()
        {
            return this;
        }

        /// <summary>
        /// Syntactic sugar (for now) to help indicate the start of the 'Act' part of a test.
        /// </summary>
        /// <returns>The current <see cref="RequestSpecification"/>.</returns>
        public RequestSpecification When()
        {
            return this;
        }

        /// <summary>
        /// Performs an HTTP GET.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP GET request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Get(string endpoint)
        {
            return this.Send(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Performs an HTTP POST.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP POST request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Post(string endpoint)
        {
            return this.Send(HttpMethod.Post, endpoint);
        }

        /// <summary>
        /// Performs an HTTP PUT.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP PUT request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Put(string endpoint)
        {
            return this.Send(HttpMethod.Put, endpoint);
        }

        /// <summary>
        /// Performs an HTTP PATCH.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP PATCH request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Patch(string endpoint)
        {
            return this.Send(HttpMethod.Patch, endpoint);
        }

        /// <summary>
        /// Performs an HTTP DELETE.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP DELETE request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Delete(string endpoint)
        {
            return this.Send(HttpMethod.Delete, endpoint);
        }

        /// <summary>
        /// Implements Dispose() method of IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            this.Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implements Dispose(bool) method of IDisposable interface.
        /// </summary>
        /// <param name="disposing">Flag indicating whether objects should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.request.Dispose();
            this.disposed = true;
        }

        /// <summary>
        /// Sends the request object to the <see cref="HttpRequestProcessor"/>.
        /// </summary>
        /// <param name="httpMethod">The HTTP method to use in the request.</param>
        /// <param name="endpoint">The endpoint to be used in the request.</param>
        /// <returns>An object representing the HTTP response corresponding to the request.</returns>
        private VerifiableResponse Send(HttpMethod httpMethod, string endpoint)
        {
            // Set the HTTP method for the request
            this.request.Method = httpMethod;

            // Replace any path parameter placeholders that have been specified with their values
            if (this.pathParams.Count > 0)
            {
                StubbleVisitorRenderer renderer = new StubbleBuilder().Build();
                endpoint = renderer.Render(endpoint, this.pathParams);
            }

            // Add any query parameters that have been specified and create the endpoint
            endpoint = QueryHelpers.AddQueryString(endpoint, this.queryParams);

            this.request.RequestUri = new Uri(endpoint);

            // Set the request body using the content, encoding and content type specified
            string requestBodyAsString = this.Serialize(this.requestBody);

            this.request.Content = new StringContent(requestBodyAsString, this.contentEncoding, this.contentTypeHeader);

            // Send the request and return the result
            Task<VerifiableResponse> task = HttpRequestProcessor.Send(this.request);
            return task.Result;
        }

        /// <summary>
        /// Serializes the request body set for the request object to JSON, if necessary.
        /// </summary>
        /// <param name="body">The request body object.</param>
        /// <returns>Either the body itself (if the body is a string), or a serialized version of the body.</returns>
        private string Serialize(object body)
        {
            if (body.GetType() == typeof(string))
            {
                return (string)body;
            }
            else
            {
                return JsonConvert.SerializeObject(body);
            }
        }
    }
}
