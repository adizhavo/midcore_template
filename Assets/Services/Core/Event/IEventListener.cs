namespace Services.Core.Event
{
    public interface IEventListener <T>
    {
        void Receive(string eventId, T value);
    }
}