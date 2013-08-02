using System.Collections.Generic;
using System.Xml.Linq;
using ServerApp.GlimpseMockup.Model;

namespace ServerApp.GlimpseMockup
{
    public class GlimpseAppStorage
    {
        public GlimpseAppStorage()
        {
            RootElements = new List<XElement>();
        }

        public GlimpseModelPipeline RootPipeline { get; set; }

        public IList<XElement> RootElements { get; private set; }
    }
}
