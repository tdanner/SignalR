using Owin;
using ServerApp.GlimpseMockup;
using ServerApp.GlimpseMockup.Middleware;
using ServerApp.GlimpseMockup.Model;

namespace Owin
{
    public static class GlimpseExtensions
    {
        public static IAppBuilder WithGlimpse(this IAppBuilder app)
        {
            var context = new GlimpseAppStorage
            {
                RootPipeline = new GlimpseModelPipeline()
            };

            app.Use<GlimpseLeadingMiddleware>(new GlimpseLeadingOptions
            {
                Storage = context,
            });

            return new GlimpseAppBuilder(app, context, context.RootPipeline);
        }
    }
}
