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
    public class SolicitudSolicitarIntervencion : ISolicitud
    {
        public Solicitud TipoSolicitud { get; set; }
        public string CodigoRecogida { get; set; }

        public SolicitudSolicitarIntervencion(Solicitud solicitud, string codigo)
        {
            TipoSolicitud = solicitud;
            CodigoRecogida = codigo;
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
