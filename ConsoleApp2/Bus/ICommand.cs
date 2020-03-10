using System;

namespace ConsoleApp2
{
    /// <summary>
    /// The Command interface
    /// </summary>
    public interface ICommand : IMessage
    {
        /// <summary>
        /// Identificativo univoco del comando.
        /// </summary>
        int UidCommand { get; set; }
        /// <summary>
        /// Timestamp di emissione del comando.
        /// </summary>
        DateTime TimeStamp { get; set; }
    }
}