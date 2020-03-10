namespace ConsoleApp2
{
    /// <summary>
    /// Handler for ICommand interface
    /// </summary>
    /// <typeparam name="T">Command type to handle</typeparam>
    public interface ICommandHandler<in T> : IHandler<T> where T : ICommand
    {
    }
}