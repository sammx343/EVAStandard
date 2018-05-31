using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudPagoDatafono : ISolicitud
    {
        public string ValorEntrada { get; private set; }

        public Solicitud TipoSolicitud
        {
            get;

            set;
        }

        public SolicitudPagoDatafono(Solicitud solicitud, string valorEntrada)
        {
            TipoSolicitud = solicitud;
            ValorEntrada = valorEntrada;
        }

        public override string ToString()
        {
            string ans = "";
            if (this != null)
            {
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(TipoSolicitud.ToDescriptionString(), Newtonsoft.Json.Formatting.None);
            }
            return ans;
        }
    }
}
