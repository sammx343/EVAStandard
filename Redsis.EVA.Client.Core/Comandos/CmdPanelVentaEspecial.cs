using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelVentaEspecial : ComandoAbstract
    {
        Solicitudes.SolicitudPanelVentaEspecial Solicitud;

        public CmdPanelVentaEspecial(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as Solicitudes.SolicitudPanelVentaEspecial;
        }

        public override void Ejecutar()
        {
            Telemetria.Instancia.AgregaMetrica(new Evento("PanelVentaEspecial"));

            log.Info("[CmdPanelVentaEspecial] Mostrando modal tipo venta.");

            //
            iu.MostrarModalVentaEspecial();
            iu.PanelOperador.MensajeOperador = Solicitud.CodigoTipoVenta;
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
