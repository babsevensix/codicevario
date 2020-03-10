namespace ConsoleApp1.Entity
{
    public interface IEntityConStatoLavorazione
    {
        int IdStatoLavorazione { get; set; }

        StatoLavorazione LinkStatoLavorazione { get; set; }
    }
}