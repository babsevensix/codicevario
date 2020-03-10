namespace ConsoleApp2
{
    /// <summary>
    /// Interfaccia implementata dalle regole in grado di fornire
    /// un oggetto associato di tipo IRuleProgressModelBuilder
    /// (ovvero un oggetto in grado di calcolare lo stato di avanzamento
    /// della regola).
    /// </summary>
    public interface IRuleProgressModelBuilderProvider
    {
        /// <summary>
        /// Restituisce una istanza di tipo IRuleProgressModelBuilder
        /// in grado di restituire lo stato di avanzamento rispetto alla regola.
        /// </summary>
        /// <returns></returns>
        IRuleProgressModelBuilder GetRuleProgressModelBuilder();
    }
}