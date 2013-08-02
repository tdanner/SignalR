using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerApp.GlimpseMockup.Middleware
{
    public class GlimpseTrailingMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly GlimpseTrailingOptions _options;

        public GlimpseTrailingMiddleware(
            Func<IDictionary<string, object>, Task> next,
            GlimpseTrailingOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            await _next(environment);
        }
    }
}