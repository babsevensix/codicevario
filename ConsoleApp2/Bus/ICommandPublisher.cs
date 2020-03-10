using System.Collections.Generic;

namespace ConsoleApp2
{
    /// <summary>
    /// ICommand publisher
    /// </summary>
    public interface ICommandPublisher
    {
        /// <summary>
        /// Publish to bus a command
        /// </summary>
        /// <typeparam name="T">Command type</typeparam>
        /// <param name="command">Command to publish</param>
        void Send<T>(T command) where T : ICommand;


        T GetCommandById<T>(int idCommand) where T : ICommand;

        IEnumerable<T> GetCommandsById<T>(string[] idCommands) where T : ICommand;
    }
}