using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using ServerApp.GlimpseMockup.Recording;

namespace ServerApp.GlimpseMockup.Middleware
{
    public class GlimpseObservingMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly GlimpseObservingOptions _options;

        public GlimpseObservingMiddleware(
            Func<IDictionary<string, object>, Task> next,
            GlimpseObservingOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var scope = new GlimpseScope();
            scope.Enter("Observing middleware");
            scope.Raise("Middleware", new XElement("middleware", new XAttribute("type", _options.Node.UseMiddleware)));
            scope.Raise("Before", Snapshot(environment));
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
                scope.Raise("After", Snapshot(environment));
                scope.Exit();
            }
        }

        private XElement Snapshot(IDictionary<string, object> environment)
        {
            var snapshot = new XElement("environment");
            foreach (var kv in environment)
            {
                var item = new XElement("item", new XAttribute("name", kv.Key));
                snapshot.Add(item);
                if (kv.Value == null)
                {
                    continue;
                }
                var headers = kv.Value as IDictionary<string, string[]>;
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (header.Value != null)
                        {
                            foreach (var value in header.Value)
                            {
                                item.Add(new XElement("header", new XAttribute("name", header.Key), new XText(value)));
                            }
                        }
                    }
                }
                else if (kv.Value.GetType().IsValueType || kv.Value is string)
                {
                    item.Add(Convert.ToString(kv.Value));
                }
            }
            return snapshot;
        }
    }
}