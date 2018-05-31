using Redsis.EVA.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EClientes
    {

        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<ECliente> ListaClientes { get; private set; }

        public EClientes()
        {
            ListaClientes = new List<ECliente>();
        }

        public ECliente Cliente(string id, out Respuesta respuesta)
        {
            respuesta = new Respuesta();
            ECliente cliente = ListaClientes.Find(x => String.Equals(id, x.Id, StringComparison.Ordinal));
            if (cliente != null)
            {
                respuesta.Valida = true;
                return cliente;
            }
            else
            {
                log.Info("[Cliente]: Cliente no encontrado.");
                respuesta.Documentar(false, "[Cliente]: Cliente no encontrado.");
                return null;
                //throw new Exception("Cliente no encontrado.");
            }
        }

    }
}
