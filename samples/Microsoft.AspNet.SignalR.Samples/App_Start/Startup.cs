using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using System.Threading;
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

            app.Map("/signalr", map =>
            {
                map = map.WithGlimpse();
                map.RunSignalR(config);
            });

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

        //private class RequestMiddleware : OwinMiddleware
        //{
        //    public RequestMiddleware(OwinMiddleware next)
        //        : base(next)
        //    {
        //    }

        //    public override Task Invoke(IOwinContext context)
        //    {
        //        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\abnanda\Desktop\SignalR\SignalR\LPRequest.txt", true))
        //        {
        //            file.WriteLine("Request : ");
        //            file.WriteLine(context.Request.Method);
        //            file.WriteLine(context.Request.Path);
        //            file.WriteLine(context.Request.QueryString);
        //            file.WriteLine(context.Request.PathBase);

        //            file.WriteLine("Response : ");
        //            file.WriteLine(context.Response.StatusCode);
        //            file.WriteLine(context.Response.ReasonPhrase);

        //            file.WriteLine(" \n ");

        //            return Next.Invoke(context);
        //        }
        //    }
        //}

        //private class Test
        //{
        //    public string Data { get; set; }
        //}

        //private async Task TraceRequest(IOwinContext context, Func<Task> next)
        //{
        //    IOwinRequest request = context.Request;
        //    IOwinResponse response = context.Response;

        //    var test = new Test() { Data = "bar!" + Thread.CurrentThread.ManagedThreadId };

        //    CallContext.LogicalSetData("foo", test);

        //    await Task.Factory.StartNew(() =>
        //    {
        //        var innerTest = (Test)CallContext.LogicalGetData("foo");

        //        Trace.WriteLine(string.Format("foo startnew {0} {1} ",
        //            Thread.CurrentThread.ManagedThreadId,
        //            innerTest.Data
        //            ));

        //        innerTest.Data = "bar???" + Thread.CurrentThread.ManagedThreadId;
        //    });

        //    var newTest = (Test)CallContext.LogicalGetData("foo");
        //    Trace.WriteLine(string.Format("foo in {0} {1} ", Thread.CurrentThread.ManagedThreadId, newTest.Data));

        //    var output = context.Get<TextWriter>("host.TraceOutput");

        //    output.WriteLine(
        //        "{0} {1}{2} {3}",
        //        request.Method, request.PathBase, request.Path, request.QueryString);

        //    await next();

        //    newTest = (Test)CallContext.LogicalGetData("foo");

        //    Trace.WriteLine(string.Format("foo out {0} {1} ", Thread.CurrentThread.ManagedThreadId, newTest.Data));

        //    output.WriteLine(
        //        "{0}",
        //        response.StatusCode);
        //}
    }
}