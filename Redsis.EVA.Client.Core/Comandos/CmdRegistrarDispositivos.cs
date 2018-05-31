using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdRegistrarDispositivos : ComandoAbstract
    {
        Solicitudes.SolicitudRegistrarDispositivos Solicitud;

        public CmdRegistrarDispositivos(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudRegistrarDispositivos;
        }

        public override void Ejecutar()
        {
            Telemetria.Instancia.AgregaMetrica(new Evento("RegistrarDispositivos"));

            log.DebugFormat("[CmdRegistrarPerifericos] {0}", Solicitud.ToString());
            iu.PanelDispositivo.MostrarTitulo("Registrando dispositivos...");
            
            Respuesta respuesta = new Respuesta();
            iu.PanelDispositivo.RegistrarDispositivos(out respuesta);           
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
