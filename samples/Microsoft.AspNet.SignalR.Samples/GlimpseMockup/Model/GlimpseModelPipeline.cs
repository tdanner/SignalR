using System.Collections.Generic;
using System.Linq;

namespace ServerApp.GlimpseMockup.Model
{
    public class GlimpseModelPipeline
    {
        public GlimpseModelPipeline()
        {
            Nodes = new List<GlimpseModelNode>();
        }
        public GlimpseModelPipeline Parent { get; set; }
        public IList<GlimpseModelNode> Nodes { get; private set; }
    }
}