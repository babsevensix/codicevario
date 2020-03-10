using System;

namespace ConsoleApp1.Entity
{
    public class StatoLavorazione : IEntity
    {
        public int Id { get; set; } 
        public int IdAccountInserimento { get; set; }

        public DateTime DataInserimento { get; set; }
        public int? IdAccountVisualizzazione { get; set; }
        public DateTime? DataVisualizzazione { get; set; }
        
        public int? IdAccountPresaInCarico { get; set; }
        public DateTime? DataPresaInCarico { get; set; }

        public int? IdAccountLavorata { get; set; }
        public DateTime? DataLavorazione { get; set; }


        public virtual Account LinkAccountInserimento { get; set; }
        public virtual Account LinkAccountVisualizzazione { get; set; }
        public virtual Account LinkAccountPresaInCarico { get; set; }
        public virtual Account LinkAccountLavorata { get; set; }


        
    }
}