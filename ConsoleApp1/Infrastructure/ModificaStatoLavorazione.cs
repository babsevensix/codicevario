using System;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleApp1.Entity;

namespace ConsoleApp1.Infrastructure
{
    public class ModificaStatoLavorazione<TProperty>
    {
        private readonly StatoLavorazione _statoLavorazione;
        private readonly Expression<Func<StatoLavorazione, TProperty>> _selector;
        

        public ModificaStatoLavorazione(StatoLavorazione statoLavorazione, Expression<Func<StatoLavorazione, TProperty>> selector)
        {
            _statoLavorazione = statoLavorazione;
            _selector = selector;
        }

      
        public StatoLavorazione Il(DateTime quando)
        {
            var prop = (PropertyInfo)((MemberExpression)_selector.Body).Member;
            prop.SetValue(_statoLavorazione, quando, null);
            return _statoLavorazione;
        }

        public StatoLavorazione Ora()
        {
            var prop = (PropertyInfo)((MemberExpression)_selector.Body).Member;
            prop.SetValue(_statoLavorazione, DateTime.Now, null);
            return _statoLavorazione;
        }
    }
}