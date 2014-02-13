// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hosting;

namespace Microsoft.AspNet.SignalR
{
    /// <summary>
    /// Represents a SignalR request
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Gets the url for this request.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// The local path part of the url
        /// </summary>
        string LocalPath { get; }
        
        /// <summary>
        /// Gets the querystring for this request.
        /// </summary>
        INameValueCollection QueryString { get; }

        /// <summary>
        /// Get the content type from the header.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Get the content length from the header.
        /// </summary>
        int ContentLength { get; }

        /// <summary>
        /// Gets the headers for this request.
        /// </summary>
        INameValueCollection Headers { get; }

        /// <summary>
        /// Gets the cookies for this request.
        /// </summary>
        IDictionary<string, Cookie> Cookies { get; }

        /// <summary>
        /// Gets security information for the current HTTP request.
        /// </summary>
        IPrincipal User { get; }

        /// <summary>
        /// Gets the owin enviornment
        /// </summary>
        IDictionary<string, object> Environment { get; }

        /// <summary>
        /// Reads the form of the http request when content-type is application/x-www-form-urlencoded
        /// </summary>
        /// <returns></returns>
        Task<INameValueCollection> ReadForm();

        /// <summary>
        /// Read raw binary data sent when the content-type is application/octet-stream
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ReadRawData();
    }
}
