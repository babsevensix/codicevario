namespace ConsoleApp2
{
    /// <summary>
    /// Publisher event interface
    /// </summary>
    public interface IEventPublisher
    {
        
        void Publish<T>(T @event, string raiseFromFullClassName) where T : IEvent;
    }
}