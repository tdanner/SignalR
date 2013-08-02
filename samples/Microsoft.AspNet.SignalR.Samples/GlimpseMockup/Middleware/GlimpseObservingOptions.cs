using ServerApp.GlimpseMockup.Model;

namespace ServerApp.GlimpseMockup.Middleware
{
    public class GlimpseObservingOptions
    {
        public GlimpseAppStorage Storage { get; set; }
        public GlimpseModelPipeline Pipeline { get; set; }
        public GlimpseModelNode Node { get; set; }
    }
}