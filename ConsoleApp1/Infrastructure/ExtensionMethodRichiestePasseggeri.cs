using System;
using ConsoleApp1.Entity;

namespace ConsoleApp1.Infrastructure
{
    public static class ExtensionMethodRichiestePasseggeri
    {
        #region Stato Lavorazione
        public static ModificaStatoLavorazione<DateTime?> VisualizzatoDa(this StatoLavorazione statoLavorazione, Account account)
        {
            statoLavorazione.IdAccountVisualizzazione = account.Id;
            statoLavorazione.LinkAccountVisualizzazione = account;
            ModificaStatoLavorazione<DateTime?> msl = new ModificaStatoLavorazione<DateTime?>(statoLavorazione, sl=>sl.DataVisualizzazione);
            return msl;
        }

        
        public static ModificaStatoLavorazione<DateTime?> PresoInCaricoDa(this StatoLavorazione entity, Account account)
        {
            entity.IdAccountPresaInCarico = account.Id;
            entity.LinkAccountPresaInCarico = account;
            ModificaStatoLavorazione<DateTime?> msl = new ModificaStatoLavorazione<DateTime?>(entity, sl => sl.DataPresaInCarico);
            return msl;
        }
        public static ModificaStatoLavorazione<DateTime?> LavorataDa(this StatoLavorazione entity, Account account)
        {
            entity.IdAccountLavorata = account.Id;
            entity.LinkAccountLavorata = account;
            ModificaStatoLavorazione<DateTime?> msl = new ModificaStatoLavorazione<DateTime?>(entity, sl => sl.DataLavorazione);
            return msl;
        }


        public static ModificaStatoLavorazione<DateTime?> InseritaDa(this StatoLavorazione statoLavorazione, Account account)
        {
            statoLavorazione.IdAccountInserimento = account.Id;
            statoLavorazione.LinkAccountInserimento = account;
            ModificaStatoLavorazione<DateTime?> msl = new ModificaStatoLavorazione<DateTime?>(statoLavorazione, sl => sl.DataInserimento);
            return msl;
        }

        #endregion


        #region IEntityConStatoLavorazione

        

        

        public static ModificaStatoLavorazione<DateTime?> VisualizzatoDa(this IEntityConStatoLavorazione entity, Account account)
        {
            entity.VisualizzatoDa(account);
            ModificaStatoLavorazione<DateTime?> msl = new ModificaStatoLavorazione<DateTime?>(entity.LinkStatoLavorazione, sl => sl.DataVisualizzazione);
            return msl;
        }

        public static ModificaStatoLavorazione<DateTime?> PresoInCaricoDa(this IEntityConStatoLavorazione entity, Account account)
        {
            entity.PresoInCaricoDa(account);
            ModificaStatoLavorazione<DateTime?> msl = new ModificaStatoLavorazione<DateTime?>(entity.LinkStatoLavorazione, sl => sl.DataPresaInCarico);
            return msl;
        }

        public static ModificaStatoLavorazione<DateTime?> LavorataDa(this IEntityConStatoLavorazione entity, Account account)
        {
            entity.LavorataDa(account);
            ModificaStatoLavorazione<DateTime?> msl = new ModificaStatoLavorazione<DateTime?>(entity.LinkStatoLavorazione, sl => sl.DataLavorazione);
            return msl;
        }


        public static ModificaStatoLavorazione<DateTime> InseritaDa(this IEntityConStatoLavorazione entity, Account account)
        {
            entity.InseritaDa(account);
            ModificaStatoLavorazione<DateTime> msl = new ModificaStatoLavorazione<DateTime>(entity.LinkStatoLavorazione, sl => sl.DataInserimento);
            return msl;
        }

        #endregion
    }
}