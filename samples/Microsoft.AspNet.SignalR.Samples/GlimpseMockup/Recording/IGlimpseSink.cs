namespace ServerApp.GlimpseMockup.Recording
{
    public interface IGlimpseSink
    {
        void Enter(GlimpseScope scope);
        void Exit(GlimpseScope scope);
        void Raise(GlimpseMessage message);
    }
}