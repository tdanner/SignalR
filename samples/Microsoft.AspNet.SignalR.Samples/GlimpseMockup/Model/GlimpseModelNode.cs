using System.Collections.Generic;

namespace ServerApp.GlimpseMockup.Model
{
    public class GlimpseModelNode
    {
        public GlimpseModelNode()
        {
            NewPipelines = new List<GlimpseModelPipeline>();
        }

        public GlimpseModelPipeline Pipeline { get; set; }
        public List<GlimpseModelPipeline> NewPipelines { get; private set; }

        public object UseMiddleware { get; set; }
        public object UseArgs { get; set; }
    }
}