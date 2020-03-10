using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp2.Rules.Login.Commands;
using NLog;

namespace ConsoleApp2
{
    public class LoginRuleHandler : AbstractRuleHandler<LoginRuleDefinition>,
            ICommandHandler<LoginCommand>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override string GetAssemblyQualifyNameOfCommandHandler()
        {
            return typeof(LoginCommand).AssemblyQualifiedName;
        }

        public LoginRuleHandler(IDbContext dbContext, IProcessBus bus, IAccountRepository accountRepository) 
            : base(dbContext, bus, accountRepository)
        {
        }

        /// <summary>
        /// Auto register 
        /// </summary>
        /// <param name="bus">process bus</param>
        /// <param name="executingLostEvent"></param>
        public void AutoRegisterIntoBus(IProcessBus bus, bool disableAutoReLaunchEvents = false)
        {
            var channelName = GetType().AssemblyQualifiedName;
            bus.RegisterHandler<LoginCommand>(Handle, channelName);
        }

        /// <summary>
        /// Handler of login command
        /// </summary>
        /// <param name="command"></param>
        public void Handle(LoginCommand command)
        {
            Logger.Debug($"Handling LoginCommand with Uid={command.UidCommand} (AccountId = {command.AccountId})");

            var listRuleRegistered = GetRulesActiveIn(command.TimeStamp);
            foreach (var rule in listRuleRegistered.Keys)
            {
                var idCampaign = listRuleRegistered[rule];
                Logger.Debug($"Considering rule with Uid={rule.UidRule} for campaign {idCampaign}.");
                var campaign = _dbContext.Query<Campaign>().Single(x => x.Id == idCampaign);
                if (HasUserAlreadyAccomplishedRule(campaign.UidCampaign, rule.UidRule, command.AccountId))
                {
                    Logger.Debug($"Rule already accomplished.");
                    continue;
                }

                if (!AccomplishedCampaignRules(command, campaign))
                {
                    Logger.Debug("Base campaign rules not satisfied. Rule not accomplished.");
                    continue;
                }
                if (! command.LoginPerformedInRuleDate(rule) )
                { 
                    Logger.Debug($"-> login performed out of required date range.");
                    continue;
                }
                Logger.Debug($"-> login valid for rule.");
                RaiseAccomplishedEvent(command.AccountId, campaign.UidCampaign, rule.UidRule, command.LoginDateTime,
                    command.UidCommand);
                _dbContext.Commit();
            }
        }

      

       
       
    }
}
