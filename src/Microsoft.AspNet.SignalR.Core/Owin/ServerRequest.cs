﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.Owin;
using System.IO;

namespace Microsoft.AspNet.SignalR.Owin
{
    public class ServerRequest : IRequest
    {
        private INameValueCollection _queryString;
        private INameValueCollection _headers;
        private IDictionary<string, Cookie> _cookies;

        private readonly OwinRequest _request;

        public ServerRequest(IDictionary<string, object> environment)
        {
            _request = new OwinRequest(environment);
        }

        public Uri Url
        {
            get
            {
                return _request.Uri;
            }
        }

        public string LocalPath
        {
            get
            {
                return (_request.PathBase.ToString() + _request.Path.ToString());
            }
        }

        public string ContentType
        {
            get
            {
                return Headers["Content-Type"];
            }
        }

        public int ContentLength
        {
            get
            {
                var contentLength = Headers["Content-Length"];
                return int.Parse(contentLength);
            }
        }

        public INameValueCollection QueryString
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _queryString, () =>
                    {
                        return new ReadableStringCollectionWrapper(_request.Query);
                    });
            }
        }

        public INameValueCollection Headers
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _headers, () =>
                    {
                        return new ReadableStringCollectionWrapper(_request.Headers);
                    });
            }
        }

        public IDictionary<string, Cookie> Cookies
        {
            get
            {
                return LazyInitializer.EnsureInitialized(
                    ref _cookies, () =>
                    {
                        var cookies = new Dictionary<string, Cookie>(StringComparer.OrdinalIgnoreCase);
                        foreach (var kv in _request.Cookies)
                        {
                            if (!cookies.ContainsKey(kv.Key))
                            {
                                cookies.Add(kv.Key, new Cookie(kv.Key, kv.Value));
                            }
                        }
                        return cookies;
                    });
            }
        }

        public IPrincipal User
        {
            get { return _request.User; }
        }

        public IDictionary<string, object> Environment
        {
            get
            {
                return _request.Environment;
            }
        }

        public async Task<INameValueCollection> ReadForm()
        {
            IFormCollection form = await _request.ReadFormAsync();
            return new ReadableStringCollectionWrapper(form);
        }

        public Task<byte[]> ReadRawData()
        {
            //var contentLength = ContentLength;
            //byte[] data;
            //using (BinaryReader reader = new BinaryReader(Environment.Get<Stream>(OwinConstants.RequestBody)))
            //{
            //    data = reader.ReadBytes(contentLength);
            //}

            //return data;

            return Task<byte[]>.Factory.StartNew(() =>
            {
                byte[] data;
                using (BinaryReader reader = new BinaryReader(Environment.Get<Stream>(OwinConstants.RequestBody)))
                {
                    data = reader.ReadBytes(ContentLength);
                }

                return data;
            });
        }
    }
}
