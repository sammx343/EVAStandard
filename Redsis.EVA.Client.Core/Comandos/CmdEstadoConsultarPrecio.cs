using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdEstadoConsultarPrecio : ComandoAbstract
    {
        public Solicitudes.SolicitudEstadoConsultarPrecio Solicitud { get; set; }

        public CmdEstadoConsultarPrecio(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudEstadoConsultarPrecio;
        }

        public override void Ejecutar()
        {
            Telemetria.Instancia.AgregaMetrica(new Evento("EstadoConsutarPrecio"));

            log.Info("[CmdEstadoConsultarPrecio] Cambio de estado para consulta de precios.");
        }

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }
    }
}
