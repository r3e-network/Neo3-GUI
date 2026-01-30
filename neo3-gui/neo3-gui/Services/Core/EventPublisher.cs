using System.Collections.Concurrent;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// Event publisher service implementation
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var handlers = _handlers.GetOrAdd(typeof(T), _ => new List<Delegate>());
            lock (handlers)
            {
                handlers.Add(handler);
            }
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (_handlers.TryGetValue(typeof(T), out var handlers))
            {
                lock (handlers)
                {
                    handlers.Remove(handler);
                }
            }
        }

        public void Publish<T>(T eventData)
        {
            if (_handlers.TryGetValue(typeof(T), out var handlers))
            {
                List<Delegate> copy;
                lock (handlers)
                {
                    copy = new List<Delegate>(handlers);
                }
                foreach (var handler in copy)
                {
                    ((Action<T>)handler)(eventData);
                }
            }
        }
    }
}
