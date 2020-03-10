using System;

namespace ConsoleApp2
{
    public class LoginCommand : AbstractAccountCommand
    {
        /// <summary>
        /// Date and time of user's login.
        /// </summary>
        public DateTime LoginDateTime { get; set; }

        public LoginCommand(int uidCommand, int accountId, 
            EnumChannels channel, DateTime loginTimestamp) : base(uidCommand, accountId)
        {
            EnumChannel = channel;
            LoginDateTime = loginTimestamp;
        }
    }
}