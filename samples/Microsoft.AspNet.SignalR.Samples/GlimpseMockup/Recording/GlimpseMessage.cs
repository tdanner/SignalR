namespace ServerApp.GlimpseMockup.Recording
{
    public class GlimpseMessage
    {
        public GlimpseScope Scope { get; set; }

        public long RaiseAtElapsedTicks { get; set; }

        public string Note { get; set; }

        public object Data { get; set; }
    }
}
