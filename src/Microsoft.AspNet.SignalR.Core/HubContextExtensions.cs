using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR
{
    public static class HubContextExtensions
    {
        public static IDisposable Subscribe(this IHubContext context, Func<ClientHubInvocation, Task> callback)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            return context.Connection.Receive(async message =>
            {
                // TODO: Use JsonSerializer from the DR
                var invocation = JsonConvert.DeserializeObject<ClientHubInvocation>(message);

                await callback(invocation);
            });
        }

        public static IDisposable On(this IHubContext context, string method, Func<Task> callback)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            return context.Subscribe(async invocation =>
            {
                if (invocation.Method == method)
                {
                    await callback();
                }
            });
        }
    }
}
