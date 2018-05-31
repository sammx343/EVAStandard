using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ETiposAjuste
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<ETipoAjuste> TiposAjuste { get; private set; }

        public ETiposAjuste()
        {
            TiposAjuste = new List<ETipoAjuste>();
        }

        public ETipoAjuste Ajuste(string id)
        {
            ETipoAjuste ajuste = TiposAjuste.Find(x => String.Equals(id, x.Id, StringComparison.Ordinal));
            if (ajuste != null)
            {
                return ajuste;
            }
            else
            {
                log.Info("[TipoAjuste]: Tipo de Ajuste no encontrado.");
                throw new Exception("Tipo de ajuste no encontrado.");
            }
        }
    }
}
