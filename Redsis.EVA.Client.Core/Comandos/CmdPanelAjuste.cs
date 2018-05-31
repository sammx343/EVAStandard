using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelAjuste : ComandoAbstract
    {
        Solicitudes.SolicitudPanelAjuste solicitud;

        public CmdPanelAjuste(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudPanelAjuste;
        }

        public override void Ejecutar()
        {
            Telemetria.Instancia.AgregaMetrica(new Evento("PanelAjustes"));

            log.Info("[CmdPanelAjustes] Mostrando modal ajustes");
            //iu.mostraraj();
            iu.MostrarModalAjustes();
            //iu.PanelOperador.MensajeOperador = Solicitud.CodigoRecogida;
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
