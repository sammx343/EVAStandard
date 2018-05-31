using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Entidades;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudPagarVenta : ISolicitud
    {
        public string ValorEntrada { get; private set; }

        public EPago Pago { get; private set; }

        public Solicitud TipoSolicitud
        {
            get;

            set;
        }
        
        public SolicitudPagarVenta(Solicitud solicitudId, string valorEntrada)
        {
            ValorEntrada = valorEntrada;
            TipoSolicitud = solicitudId;
        }

        public SolicitudPagarVenta(Solicitud solicitud, EPago pago)
        {
            Pago = pago;
            TipoSolicitud = solicitud;
        }



        public override string ToString()
        {
            string ans = "";
            if(this != null)
            {
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(TipoSolicitud.ToDescriptionString(), Newtonsoft.Json.Formatting.None);
            }
            return ans;
        }
    }
}
