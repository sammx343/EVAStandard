using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdLimpiarVisor : ComandoAbstract
    {
        Solicitudes.SolicitudVolver solicitudVolver;

        public CmdLimpiarVisor(ISolicitud solicitud) : base(solicitud)
        {
            solicitudVolver = solicitud as Solicitudes.SolicitudVolver;
        }

        public override void Ejecutar()
        {
            iu.PanelVentas.VisorCliente.LimpiarArticulo();
            iu.PanelVentas.VisorMensaje = "";
            iu.PanelOperador.MensajeOperador = "";

            Telemetria.Instancia.AgregaMetrica(new Evento("LimpiarVisor"));

            log.Info("[CmdLimpiarVisor] Limpiando Visor.");
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
