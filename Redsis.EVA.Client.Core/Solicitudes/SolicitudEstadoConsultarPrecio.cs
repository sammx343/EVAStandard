using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudEstadoConsultarPrecio : ISolicitud
    {
        public SolicitudEstadoConsultarPrecio(Solicitud solicitud)
        {
            TipoSolicitud = solicitud;
        }

        public Solicitud TipoSolicitud
        {
            get;

            set;
        }

        public override string ToString()
        {
            string ans = "";
            if (this != null)
            {
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(TipoSolicitud.ToDescriptionString(), Newtonsoft.Json.Formatting.Indented);
            }
            return ans;
        }
    }
}
