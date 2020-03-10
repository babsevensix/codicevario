namespace ConsoleApp2
{
    public interface IRuleCampaignDefinition : IRuleDefinition
    {
        /// <summary>
        /// Restituisce una descrizione testuale della regola.
        /// [Returns a textual description of the rule.]
        /// </summary>
        /// <returns></returns>
        string GetHumanDescription();

        /// <summary>
        /// Restituisce il nome della regola.
        /// [Returns the rule name.]
        /// </summary>
        /// <returns></returns>
        string GetRuleName();

        /// <summary>
        /// Restituisce il tipo della regola (come enum).
        /// [Returns the rule type as an enum.]
        /// </summary>
        /// <returns></returns>
        EnumCampaignRuleType GetRuleTypeEnum();

        /// <summary>
        /// Restituisce il tipo della regola (id dell'enum).
        /// [Returns the rule type as an enum id.]
        /// </summary>
        /// <returns></returns>
        int GetRuleTypeEnumId();

        /// <summary>
        /// Restituisce l'eventuale codice promo associato alla regola
        /// (o null se non applicabile).
        /// </summary>
        /// <returns></returns>
        string GetPromoCode();
    }
}