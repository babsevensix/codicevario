using System;

namespace ConsoleApp2
{
    /// <summary>
    /// Login rule definition
    /// </summary>
    public class LoginRuleDefinition : AbstractRuleDefinitionWithRecurrency, IRuleProgressModelBuilderProvider
    {

        public LoginRuleDefinition(int uidRule,
            DateTime startDate, DateTime? endDate) : base(uidRule, startDate, endDate, null)
        {
        }

        public override string GetHumanDescription()
        {
            var desc = "Per completare questo passo è necessario effettuare il login";
            if (EndDate.HasValue) desc += " entro il " + EndDate.Value.ToString("dd/MM/yyyy H:mm");
            return desc;
        }

        public override string GetRuleName()
        {
            return "Login";
        }

        public override EnumCampaignRuleType GetRuleTypeEnum()
        {
            return EnumCampaignRuleType.Login;
        }

        public override int GetRuleTypeEnumId()
        {
            return (int)EnumCampaignRuleType.Login;
        }

        /// <summary>
        /// No promo code
        /// </summary>
        /// <returns></returns>
        public override string GetPromoCode()
        {
            return null;
        }

        public IRuleProgressModelBuilder GetRuleProgressModelBuilder()
        {
            throw new NotImplementedException();
        }
    }
}