namespace ConsoleApp2
{
    /// <summary>
    /// Interface to handle a message
    /// </summary>
    /// <typeparam name="T">Message to handle</typeparam>
    public interface IHandler<in T> where T : IMessage
    {
        void Handle(T message);
    }
}