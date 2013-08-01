using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Samples;
using Microsoft.AspNet.SignalR.Tests.Common;
using Microsoft.AspNet.SignalR.Tests.Common.Connections;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Microsoft.AspNet.SignalR.Samples
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\abnanda\Desktop\SignalR\SignalR\LPRequest.txt");

            //app.Use((context, next) =>
            //{
            //    file.WriteLine(context.Request.ToString() + "\n" + context.Response.ToString());
            //});

            app.MapSignalR<SendingConnection>("/sending-connection");
            app.MapSignalR<TestConnection>("/test-connection");
            app.MapSignalR<RawConnection>("/raw-connection");
            app.MapSignalR<StreamingConnection>("/streaming-connection");

            app.Use(typeof(ClaimsMiddleware));

            ConfigureSignalR(GlobalHost.DependencyResolver, GlobalHost.HubPipeline);

            var config = new HubConfiguration()
            {
                EnableDetailedErrors = true
            };

            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(30);

            app.MapSignalR(config);

            app.Map("/basicauth", map =>
            {
                map.UseBasicAuthentication(new BasicAuthenticationProvider());
                map.MapSignalR<AuthenticatedEchoConnection>("/echo");
                map.MapSignalR();
            });

            BackgroundThread.Start();
        }

        private class ClaimsMiddleware : OwinMiddleware
        {
            public ClaimsMiddleware(OwinMiddleware next)
                : base(next)
            {
            }

            public override Task Invoke(IOwinContext context)
            {
                string username = context.Request.Headers.Get("username");

                if (!String.IsNullOrEmpty(username))
                {
                    var authenticated = username == "john" ? "true" : "false";

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Authentication, authenticated)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims);
                    context.Request.User = new ClaimsPrincipal(claimsIdentity);
                }

                return Next.Invoke(context);
            }
        }

        private class RequestMiddleware : OwinMiddleware
        {
            private static System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\abnanda\Desktop\SignalR\SignalR\LPRequest.txt");

            public RequestMiddleware(OwinMiddleware next)
                :base(next)
            {
            }

            public override Task Invoke(IOwinContext context)
            {
                file.WriteLine(context.Request.ToString() + "\n" + context.Response.ToString());
                return Next.Invoke(context);
            }
        }
    }
}