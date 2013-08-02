using System.Threading;
using System.Xml.Linq;
using ServerApp.GlimpseMockup.Recording;

namespace ServerApp.GlimpseMockup.Middleware
{
    public class GlimpseAppSink : IGlimpseSink
    {
        private readonly GlimpseAppStorage _storage;
        private readonly XElement _rootElement;

        public GlimpseAppSink(GlimpseAppStorage storage)
        {
            _storage = storage;
            _rootElement = new XElement("root");
        }

        private XElement GetElement(GlimpseScope scope)
        {
            if (scope == null)
            {
                return _rootElement;
            }

            object value;
            if (!scope.Items.TryGetValue(this, out value))
            {
                return null;
            }
            return value as XElement;
        }

        public void Enter(GlimpseScope scope)
        {
            var outerElement = GetElement(scope.OuterScope);
            var scopeElement = new XElement("scope", 
                new XAttribute("note", scope.Note), 
                new XAttribute("enter-tick", scope.EnterAtElapsedTicks),
                new XAttribute("enter-thread", Thread.CurrentThread.ManagedThreadId));
            scope.Items[this] = scopeElement;
            outerElement.Add(scopeElement);
        }

        public void Exit(GlimpseScope scope)
        {
            var scopeElement = GetElement(scope);
            scopeElement.Add(
                new XAttribute("exit-tick", scope.ExitAtElapsedTicks),
                new XAttribute("exit-thread", Thread.CurrentThread.ManagedThreadId));
            if (scope.OuterScope == null)
            {
                _storage.RootElements.Add(_rootElement);
            }
        }

        public void Raise(GlimpseMessage message)
        {
            var scopeElement = GetElement(message.Scope);
            var messageElement = new XElement("message",
                new XAttribute("note", message.Note),
                new XAttribute("tick", message.Scope.Stopwatch.ElapsedTicks),
                new XAttribute("thread", Thread.CurrentThread.ManagedThreadId));
            if (message.Data is XElement)
            {
                messageElement.Add(message.Data);
            }
            scopeElement.Add(messageElement);
        }
    }
}