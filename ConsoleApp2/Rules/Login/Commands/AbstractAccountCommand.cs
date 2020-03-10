namespace ConsoleApp2
{
    public abstract class AbstractAccountCommand : AbstractCommand, IAccountCommand
    {
        public EnumChannels EnumChannel { get; set; }

        protected AbstractAccountCommand(int uidCommand, int accountId): base(uidCommand)
        {
            AccountId = accountId;
        }

        public int AccountId { get; set; }
    }
}