using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EMediosPago
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<EMedioPago> ListaMediosPago { get; private set; }

        public EMediosPago()
        {
            ListaMediosPago = new List<EMedioPago>();
        }

        public EMedioPago MedioPago(string codigo)
        {
            EMedioPago medio = ListaMediosPago.Find(x => String.Equals(codigo, x.Codigo, StringComparison.Ordinal));
            if (medio != null)
            {
                return medio;
            }
            else
            {
                log.Info("[MedioPago]: Medio de Pago no encontrado.");
                throw new Exception("Medio de Pago no encontrado.");
            }
        }

        //TO DO: Crear metodo Lista()
        /*public EMediosPago lista()
        {  
        }*/
    }
}
