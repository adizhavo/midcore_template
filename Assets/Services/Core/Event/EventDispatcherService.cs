using System.Collections.Generic;

namespace Services.Core.Event
{
    /// <summary>
    /// Will register subscribers and fire events
    /// A listener can subscribe to a global or a specific event
    /// Use Subscribe(IEventListener<T> listener) with and empty 'eventId' to subscribe to a global event
    /// </summary>

    public static class EventDispatcherService<T>
    {
        private static List<Subscriber> subscribers;

        static EventDispatcherService()
        {
            subscribers = new List<Subscriber>();
        }

        public static void Subscribe(IEventListener<T> listener, string eventId = "")
        {
            if (!IsSubscribed(listener))
            {
                var subscriber = new Subscriber(eventId, listener);
                subscribers.Add(subscriber);
            }
        }

        public static void Unsubscribe(IEventListener<T> listener)
        {
            subscribers.RemoveAll(l => l.listener.Equals(listener));
        }

        public static void Dispatch(string eventId, T value)
        {
            foreach (var subscriber in subscribers)
            {
                if (string.IsNullOrEmpty(subscriber.eventId) || string.Equals(eventId, subscriber.eventId))
                {
                    subscriber.listener.Receive(eventId, value);
                }
            }
        }

        public static bool IsSubscribed(IEventListener<T> listener)
        {
            return subscribers.Find(l => l.listener.Equals(listener)) != null;
        }

        public class Subscriber
        {
            public string eventId;
            public IEventListener<T> listener;

            public Subscriber(string eventId, IEventListener<T> listener)
            {
                this.eventId = eventId;
                this.listener = listener;
            }
        }
    }
}