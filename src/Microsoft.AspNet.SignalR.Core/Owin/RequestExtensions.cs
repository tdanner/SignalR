// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Microsoft.AspNet.SignalR
{
    internal static class RequestExtensions
    {
        internal static T Get<T>(this IDictionary<string, object> values, string key)
        {
            object value;
            return values.TryGetValue(key, out value) ? (T)value : default(T);
        }

        /// <summary>
        /// Parses the request body as a form
        /// 
        /// </summary>
        public static async Task<IFormCollection> ReadFormAsync(this OwinRequest request)
        {
            IFormCollection form;
            using (var streamReader = new StreamReader(request.Body))
            {
                string text = await streamReader.ReadToEndAsync();
                form = GetForm(text);
            }
            return form;
        }

        internal static IFormCollection GetForm(string text)
        {
            IDictionary<string, string[]> form = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            var accumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            ParseDelimited(text, new[] { '&' }, AppendItemCallback, accumulator);
            foreach (var kv in accumulator)
            {
                form.Add(kv.Key, kv.Value.ToArray());
            }
            return new FormCollection(form);
        }

        private static readonly Action<string, string, object> AppendItemCallback = (name, value, state) =>
        {
            var dictionary = (IDictionary<string, List<String>>)state;

            List<string> existing;
            if (!dictionary.TryGetValue(name, out existing))
            {
                dictionary.Add(name, new List<string>(1) { value });
            }
            else
            {
                existing.Add(value);
            }
        };

        private static void ParseDelimited(string text, char[] delimiters, Action<string, string, object> callback, object state)
        {
            var textLength = text.Length;
            var equalIndex = text.IndexOf('=');
            if (equalIndex == -1)
            {
                equalIndex = textLength;
            }
            var scanIndex = 0;
            while (scanIndex < textLength)
            {
                var delimiterIndex = text.IndexOfAny(delimiters, scanIndex);
                if (delimiterIndex == -1)
                {
                    delimiterIndex = textLength;
                }
                if (equalIndex < delimiterIndex)
                {
                    while (scanIndex != equalIndex && char.IsWhiteSpace(text[scanIndex]))
                    {
                        ++scanIndex;
                    }
                    var name = text.Substring(scanIndex, equalIndex - scanIndex);
                    var value = text.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
                    callback(
                        Uri.UnescapeDataString(name.Replace('+', ' ')),
                        Uri.UnescapeDataString(value.Replace('+', ' ')),
                        state);
                    equalIndex = text.IndexOf('=', delimiterIndex);
                    if (equalIndex == -1)
                    {
                        equalIndex = textLength;
                    }
                }
                scanIndex = delimiterIndex + 1;
            }
        }
    }
}
