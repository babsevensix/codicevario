using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Rules.Login.Commands
{
    public static class LoginCommandExtension
    {

      
        public static bool LoginPerformedInRuleDate(this LoginCommand command, LoginRuleDefinition rule)
        {
            if (command.LoginDateTime < rule.StartDate) return false;
            return !rule.EndDate.HasValue || !(rule.EndDate < command.LoginDateTime);
        }
    }
}
