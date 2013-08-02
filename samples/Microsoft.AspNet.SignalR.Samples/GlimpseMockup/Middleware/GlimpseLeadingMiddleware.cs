using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Owin;
using ServerApp.GlimpseMockup.Model;
using ServerApp.GlimpseMockup.Recording;

namespace ServerApp.GlimpseMockup.Middleware
{
    public class GlimpseLeadingMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly GlimpseLeadingOptions _options;

        public GlimpseLeadingMiddleware(
            Func<IDictionary<string, object>, Task> next,
            GlimpseLeadingOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var owinContext = new OwinContext(environment);
            if (owinContext.Request.Path == "/help")
            {
                var help = new XElement("help");

                owinContext.Response.ContentType = "text/xml";
                using (var writer = XmlWriter.Create(owinContext.Response.Body))
                {
                    writer.WriteStartElement("help");
                    foreach (var root in _options.Storage.RootElements)
                    {
                        root.WriteTo(writer);
                    }
                    help.WriteTo(writer);
                }
            }
            else
            {
                var scope = new GlimpseScope(new GlimpseAppSink(_options.Storage));
                scope.Enter("Leading middleware");
                try
                {
                    await _next(environment);
                }
                catch (Exception ex)
                {
                    scope.Raise("Error", ex);
                    throw;
                }
                finally
                {
                    scope.Exit();
                }
            }
        }

    }
}