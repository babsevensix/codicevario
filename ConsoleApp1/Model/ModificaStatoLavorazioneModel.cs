using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ModificaStatoLavorazioneModel
    {
        public ModificaStatoLavorazioneModel(int idServizio, int idAccount)
        {
            IdServizio = idServizio;
            IdAccount = idAccount;
        }

        public int IdServizio { get; private set; }
        public int IdAccount { get; private set; }
    }
}
