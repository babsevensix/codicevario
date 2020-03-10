using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NLog;

namespace ConsoleApp2
{
    public abstract class AbstractRuleHandler<T> where T : IRuleCampaignDefinition
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly IDbContext _dbContext;
        protected readonly IProcessBus _bus;
        protected readonly IAccountRepository _accountRepository;

        protected AbstractRuleHandler(IDbContext dbContext, IProcessBus bus, IAccountRepository accountRepository)
        {
            _dbContext = dbContext;
            _bus = bus;
            _accountRepository = accountRepository;
        }
        /// <summary>
        /// Inidica se l'utente ha già completato la campagna per la regola indicata
        /// </summary>
        /// <param name="uidCampaign"></param>
        /// <param name="uidRuleDefinition"></param>
        /// <param name="idAccount"></param>
        /// <returns></returns>
        public bool HasUserAlreadyAccomplishedRule(int uidCampaign, int uidRuleDefinition, int idAccount)
        {
            return _dbContext.Query<RuleCampaignAccomplished>().Any(x =>
                x.UidCampaign == uidCampaign && x.UidRuleDefinition == uidRuleDefinition
                                             && x.LinkAccountForCampaign.AccountId == idAccount);
        }

        /// <summary>
        /// Recupero dal DB di tutte le regole del tipo specificato, attive nel momento specificato.
        /// </summary>
        /// <param name="when">istante di verifica</param>
        /// <returns>elenco di coppie (regola, id campagna)</returns>
        public virtual Dictionary<T, int> GetRulesActiveIn(DateTime when)
        {
            return GetRulesActiveIn<T>(when);
        }

        /// <summary>
        /// Recupero dal DB di tutte le regole del tipo specificato, attive nel momento specificato.
        /// </summary>
        /// <typeparam name="E">tipo della regola</typeparam>
        /// <param name="when">istante di verifica</param>
        /// <returns>elenco di coppie (regola, id campagna)</returns>
        protected virtual Dictionary<E, int> GetRulesActiveIn<E>(DateTime when)
        {
            var ruleTypeName = typeof(E).AssemblyQualifiedName;
            var result = new Dictionary<E, int>();
            var rulesContainer = _dbContext.Query<RuleDefinitionCampaignContainer>()
                .Where(x =>
                    x.RuleType == ruleTypeName && x.LinkCampaign.StartFrom <= when &&
                    (!x.LinkCampaign.EndDateCampaign.HasValue || x.LinkCampaign.EndDateCampaign >= when))
                .ToList();


            foreach (var ruleContainer in rulesContainer)
            {
                var ruleDeserialized = JsonConvert.DeserializeObject<E>(ruleContainer.SerializedRuleDefinition);
                result.Add(ruleDeserialized, ruleContainer.IdCampaign);
            }
            return result;
        }

        public abstract string GetAssemblyQualifyNameOfCommandHandler();


        /// <summary>
        /// Check if command accomplishes all campaign rules :
        /// 1 Start and stop campaign date
        /// 2 Accompliched campaign channel
        /// </summary>
        /// <param name="command"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>
        public virtual bool AccomplishedCampaignRules(AbstractAccountCommand command, Campaign campaign)
        {
            if (campaign.StartFrom > command.TimeStamp)
            {
                return false;
            }
            if (campaign.EndDateCampaign.HasValue && campaign.EndDateCampaign.Value < command.TimeStamp)
            {
                return false;
            }
            if (!AccomplishedCampaignChannel(command, campaign))
                return false;

            if (!campaign.WithStartButton) return true;
            // Must check if user has pressed the start button
            var accountsForCampaign =
                _dbContext.Query<AccountsForCampaign>()
                    .FirstOrDefault(
                        x =>
                            x.AccountId == command.AccountId && x.CampaignId == campaign.Id &&
                            x.UidStartButtonCommand.HasValue);

            if (accountsForCampaign == null) return false;
            var cmdStart = _dbContext.Query<CommandWrappers>()
                .Single(x => x.UidCommand == accountsForCampaign.UidStartButtonCommand.Value);
            var cmd = JsonConvert.DeserializeObject<UserPressStartButtonCampaignCommand>(cmdStart.Body);

            //Qui si verifica se il timestamp del comando è maggiore o uguale del when del comando di start campaign
            return command.TimeStamp >= cmd.When;
        }

        /// <summary>
        /// Verifica se la regola è soddisfatta, limitatamente all'aspetto relativo
        /// al canale.
        /// In agenzia è possibile solo la registrazione.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>
        protected bool AccomplishedCampaignChannel(AbstractAccountCommand command, Campaign campaign)
        {
            return campaign.CampagnaPerAgenzia && command.EnumChannel == EnumChannels.Agenzia
                   || campaign.PartecipationChannel == EnumChannels.NotDefined
                   || (campaign.PartecipationChannel & command.EnumChannel) > 0;
        }

        /// <summary>
        /// Crea e pubblica sul process bus l'evento di regola completata.
        /// </summary>
        /// <param name="accountId">identificativo dell'utente che ha completato la regola</param>
        /// <param name="uidCampaign">id della campagna</param>
        /// <param name="uidRule">id della regola</param>
        /// <param name="whenRuleAccomplished">data/ora di completamento</param>
        /// <param name="fromUidCommand">id del comando che ha generato il completamento della regola</param>
        protected void RaiseAccomplishedEvent(int accountId, int uidCampaign, int uidRule, DateTime whenRuleAccomplished,
            int fromUidCommand)
        {
            Logger.Debug($"Raising accomplished event: accountId={accountId},uidCampaign={uidCampaign},uidRule={uidRule},whenRuleAccomplished={whenRuleAccomplished}");
            var account = _accountRepository.GetOrCreateAccount(accountId);

            var uidRuleEvent = _dbContext.GetNextSequenceValue(Common.SequenceConstants.SEQUENCE_EVENTS);
            var ruleAccomplishedEvent = new RuleAccomplishedEvent(uidRuleEvent,
                uidCampaign, uidRule, account.Id, whenRuleAccomplished, fromUidCommand);
            _bus.Publish(ruleAccomplishedEvent, GetType().AssemblyQualifiedName);
            Logger.Debug($"Raised rule accomplished event with Uid={uidRuleEvent} (AccountId={account.Id}, CampaignId={uidCampaign}, RuleId={uidRule}");
        }

        /// <summary>
        /// Crea e pubblica sul process bus l'evento di regola completata, con associato un importo.
        /// </summary>
        /// <param name="accountId">identificativo dell'utente che ha completato la regola</param>
        /// <param name="uidCampaign">id della campagna</param>
        /// <param name="uidRule">id della regola</param>
        /// <param name="whenRuleAccomplished">data/ora di completamento</param>
        /// <param name="amount">importo associato all'evento</param>
        /// <param name="fromUidCommand">id del comando che ha generato il completamento della regola</param>
        protected void RaiseAccomplishedEvent(int accountId, int uidCampaign, int uidRule, DateTime whenRuleAccomplished,
            decimal amount, int fromUidCommand)
        {
            Logger.Info(
                $"Raising rule accomplished event for accountId {accountId} on campaign uid={uidCampaign} for rule {uidRule}, with amount {amount}.");
            var account = _accountRepository.GetOrCreateAccount(accountId);
            var uidRuleEvent = _dbContext.GetNextSequenceValue(SequenceConstants.SEQUENCE_EVENTS);
            var ruleAccomplishedEvent =
                new RuleAccomplishedEvent(uidRuleEvent,
                    uidCampaign, uidRule, account.Id, whenRuleAccomplished, amount, fromUidCommand);
            _bus.Publish(ruleAccomplishedEvent, GetType().AssemblyQualifiedName);
            Logger.Debug($"Raised rule accomplished event with Uid={uidRuleEvent} (AccountId={account.Id}, CampaignId={uidCampaign}, RuleId={uidRule}, Amount={amount}");
        }

        /// <summary>
        /// Crea e pubblica sul process bus l'evento di regola completata, con associato un codice promozionale.
        /// </summary>
        /// <param name="accountId">identificativo dell'utente che ha completato la regola</param>
        /// <param name="uidCampaign">id della campagna</param>
        /// <param name="uidRule">id della regola</param>
        /// <param name="whenRuleAccomplished">data/ora di completamento</param>
        /// <param name="fromUidCommand">id del comando che ha generato il completamento della regola</param>
        /// <param name="promoCode">codice promozionale associato all'evento</param>
        protected void RaiseAccomplishedEvent(int accountId, int uidCampaign, int uidRule, DateTime whenRuleAccomplished,
            int fromUidCommand, string promoCode)
        {
            Logger.Info(
                $"Raising rule accomplished event for accountId {accountId} on campaign uid={uidCampaign} for rule {uidRule}, with promo code '{promoCode}'.");
            var account = _accountRepository.GetOrCreateAccount(accountId);
            var uidRuleEvent = _dbContext.GetNextSequenceValue(Common.SequenceConstants.SEQUENCE_EVENTS);
            var ruleWithPromoAccomplishedEvent =
                new Events.Campaign.RuleWithPromoCodeAccomplishedEvent(uidRuleEvent,
                    uidCampaign, uidRule, account.Id, whenRuleAccomplished, promoCode, fromUidCommand);
            _bus.Publish(ruleWithPromoAccomplishedEvent, GetType().AssemblyQualifiedName);
            Logger.Debug($"Raised rule accomplished event with Uid={uidRuleEvent} (AccountId={account.Id}, CampaignId={uidCampaign}, RuleId={uidRule}, Promo code='{promoCode}'");
        }

        /// <summary>
        /// Crea e pubblica sul process bus l'evento di regola completata, con associato un importo ed un codice promozionale.
        /// </summary>
        /// <param name="accountId">identificativo dell'utente che ha completato la regola</param>
        /// <param name="uidCampaign">id della campagna</param>
        /// <param name="uidRule">id della regola</param>
        /// <param name="whenRuleAccomplished">data/ora di completamento</param>
        /// <param name="fromUidCommand">id del comando che ha generato il completamento della regola</param>
        /// <param name="amount">importo associato all'evento</param>
        /// <param name="promoCode">codice promozionale associato all'evento</param>
        protected void RaiseAccomplishedEvent(int accountId, int uidCampaign, int uidRule, DateTime whenRuleAccomplished,
            int fromUidCommand, decimal amount, string promoCode)
        {
            Logger.Info(
                $"Raising rule accomplished event for accountId {accountId} on campaign uid={uidCampaign} for rule {uidRule}, with amount {amount} euro and promo code '{promoCode}'.");
            var account = _accountRepository.GetOrCreateAccount(accountId);
            var uidRuleEvent = _dbContext.GetNextSequenceValue(Common.SequenceConstants.SEQUENCE_EVENTS);
            var ruleWithPromoAccomplishedEvent =
                new Events.Campaign.RuleWithAmountAndPromoCodeAccomplishedEvent(uidRuleEvent,
                    uidCampaign, uidRule, account.Id, whenRuleAccomplished, amount, promoCode, fromUidCommand);
            _bus.Publish(ruleWithPromoAccomplishedEvent, GetType().AssemblyQualifiedName);
            Logger.Debug($"Raised rule accomplished event with Uid={uidRuleEvent} (AccountId={account.Id}, CampaignId={uidCampaign}, RuleId={uidRule}, Amount={amount} euro, Promo code='{promoCode}'");
        }

        /// <summary>
        /// Crea e pubblica sul process bus l'evento di biglietto che ha soddisfatto (completamente o parzialmente)
        /// una regola.
        /// </summary>
        /// <param name="accountId">identificativo dell'utente che ha completato la regola</param>
        /// <param name="uidCampaign">id della campagna</param>
        /// <param name="uidRule">>id della regola</param>
        /// <param name="eventDateTime">>data/ora dell'evento</param>
        /// <param name="fromUidCommand">id del comando che ha scatenato l'evento</param>
        /// <param name="ticketCode">codice AAMS del biglietto</param>
        /// <param name="ticketAmount">importo del biglietto</param>
        protected void RaiseTicketValidatedForCampaignRuleEvent(int accountId, int uidCampaign, int uidRule, 
            DateTime eventDateTime, int fromUidCommand, string ticketCode, decimal ticketAmount)
        {
            Logger.Info(
                $"Raising TicketValidatedForCampaignRuleEvent for accountId {accountId} on campaign uid={uidCampaign} for rule {uidRule}, with a ticket with code {ticketCode} and amount {ticketAmount} euro.");
            var account = _accountRepository.GetOrCreateAccount(accountId);
            var uidRuleEvent = _dbContext.GetNextSequenceValue(Common.SequenceConstants.SEQUENCE_EVENTS);
            var @event = new Events.Campaign.TicketValidatedForCampaignRuleEvent(uidRuleEvent,
                uidCampaign, uidRule, account.Id, eventDateTime, fromUidCommand, ticketCode, ticketAmount);
            _bus.Publish(@event, GetType().AssemblyQualifiedName);
            Logger.Debug($"Raised TicketValidatedForCampaignRuleEvent with Uid={uidRuleEvent} (AccountId={account.Id}, CampaignId={uidCampaign}, RuleId={uidRule}, Ticket Amount={ticketAmount} euro, Ticket code='{ticketCode}'");
        }

    }
}