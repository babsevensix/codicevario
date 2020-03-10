namespace ConsoleApp1.Entity
{
    public class Segnalazione : IEntity, IEntityConStatoLavorazione
    {
        public int Id { get; set; } 
        
        public TipoSegnalazione TipoSegnalazione { get; set; }

        public string Descrizione { get; set; }

        public int IdStatoLavorazione { get; set; }
        public virtual StatoLavorazione LinkStatoLavorazione { get; set; }


    }
}