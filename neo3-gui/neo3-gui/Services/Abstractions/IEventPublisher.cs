namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Event publisher service interface (Observer pattern)
    /// </summary>
    public interface IEventPublisher
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T eventData);
    }
}
