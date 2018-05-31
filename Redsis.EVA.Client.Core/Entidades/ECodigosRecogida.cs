using Redsis.EVA.Client.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ECodigosRecogida
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<ECodigoRecogida> CodigosRecogida { get; private set; }

        public ECodigosRecogida()
        {
            CodigosRecogida = new List<ECodigoRecogida>();
        }

        public ECodigoRecogida CodigoRecogida(string cod)
        {
            ECodigoRecogida codigo = CodigosRecogida.Find(x => String.Equals(cod, x.Codigo, StringComparison.Ordinal));
            if (codigo != null)
            {
                return codigo;
            }
            else
            {
                log.Info("[CodigoRecogida]: Codigo de Recogida no encontrado.");
                throw new Exception("Codigo de Recogida no encontrado.");
            }
        }
    }

}
