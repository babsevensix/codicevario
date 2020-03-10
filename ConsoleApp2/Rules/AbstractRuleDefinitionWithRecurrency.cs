using System;

namespace ConsoleApp2
{
    public abstract class AbstractRuleDefinitionWithRecurrency : IRuleCampaignDefinition
    {
        public AbstractRuleDefinitionWithRecurrency( int uidRule, DateTime startDate, DateTime? endDate,
            string patternEventScheduler)
        {
            UidRule = uidRule;
            StartDate = startDate;
            EndDate = endDate;
            PatternEventScheduler = patternEventScheduler;
        }
        public int UidRule { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string PatternEventScheduler { get; set; }


        public abstract string GetHumanDescription();
        public abstract string GetRuleName();
        public abstract EnumCampaignRuleType GetRuleTypeEnum();
        public abstract int GetRuleTypeEnumId();
        public abstract string GetPromoCode();
    }
}