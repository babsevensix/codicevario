using System;

namespace ConsoleApp2
{
    public abstract class AbstractCommand : ICommand
    {
        protected AbstractCommand(int uidCommand)
        {
            UidCommand = uidCommand;
            TimeStamp = DateTime.UtcNow;
        }

        public int UidCommand { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}