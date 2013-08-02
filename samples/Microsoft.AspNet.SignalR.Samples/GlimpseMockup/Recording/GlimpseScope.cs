using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace ServerApp.GlimpseMockup.Recording
{
    public class GlimpseScope
    {
        private readonly IGlimpseSink _sink;

        public GlimpseScope()
        {
            Items = new Dictionary<object, object>();
        }

        public GlimpseScope(IGlimpseSink sink)
        {
            Items = new Dictionary<object, object>();
            _sink = sink;
        }

        public static GlimpseScope Current
        {
            get { return CallContext.LogicalGetData("GlimpseScope") as GlimpseScope; }
            set { CallContext.LogicalSetData("GlimpseScope", value); }
        }

        public GlimpseScope OuterScope { get; set; }

        public string Note { get; private set; }
        public Stopwatch Stopwatch { get; private set; }
        public long EnterAtElapsedTicks { get; private set; }
        public long ExitAtElapsedTicks { get; private set; }

        public IDictionary<object,object> Items { get; private set; }

        public void Enter(string note)
        {
            Note = note;
            OuterScope = Current;
            Current = this;
            if (OuterScope != null)
            {
                Stopwatch = OuterScope.Stopwatch;
            }
            else
            {
                Stopwatch = new Stopwatch();
                Stopwatch.Start();
            }
            EnterAtElapsedTicks = Stopwatch.ElapsedTicks;

            FireEnter(this);
        }

        public void Exit()
        {
            ExitAtElapsedTicks = Stopwatch.ElapsedTicks;

            FireExit(this);

            for (var scan = Current; scan != null; scan = scan.OuterScope)
            {
                if (scan == this)
                {
                    Current = OuterScope;
                    if (OuterScope == null)
                    {
                        Stopwatch.Stop();
                    }
                    return;
                }
            }
            // warning - if you're here, there were scopes exited out of order
        }

        public void Raise(string note, object data)
        {
            var message = new GlimpseMessage
            {
                Scope = this, Note = note, RaiseAtElapsedTicks = Stopwatch.ElapsedTicks, Data = data,
            };
            FireRaise(message);
        }

        public void FireEnter(GlimpseScope scope)
        {
            if (_sink != null)
            {
                _sink.Enter(scope);
            }
            if (OuterScope != null)
            {
                OuterScope.FireEnter(scope);
            }
        }
        public void FireExit(GlimpseScope scope)
        {
            if (_sink != null)
            {
                _sink.Exit(scope);
            }
            if (OuterScope != null)
            {
                OuterScope.FireExit(scope);
            }
        }
        public void FireRaise(GlimpseMessage message)
        {
            if (_sink != null)
            {
                _sink.Raise(message);
            }
            if (OuterScope != null)
            {
                OuterScope.FireRaise(message);
            }
        }
    }
}