namespace ConsoleApp2
{
    /// <summary>
    /// Command assigned for account
    /// </summary>
    public interface IAccountCommand : ICommand
    {
        /// <summary>
        /// Account's id that generated command
        /// </summary>
        int AccountId { get; set; }
    }
}