using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Common;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdTerminalAsegurada : ComandoAbstract
    {
        Solicitudes.SolicitudTerminalAsegurada solicitudTerminalAsegurada;

        public CmdTerminalAsegurada(ISolicitud solicitud) : base(solicitud)
        {
            solicitudTerminalAsegurada = solicitud as Solicitudes.SolicitudTerminalAsegurada;
        }

        public override void Ejecutar()
        {
            if (Entorno.Instancia.Vista.PanelVentas != null)
            {
                iu.PanelVentas.VisorCliente.LimpiarArticulo();
                iu.PanelVentas.VisorMensaje = "";
                iu.PanelOperador.MensajeOperador = "";
                iu.PanelOperador.NombreUsuario = "";

                if (Config.ViewMode == InternalSettings.ModoTouch)
                {
                    iu.PanelVentas.LimpiarOperacion();
                }
            }

            Telemetria.Instancia.AgregaMetrica(new Evento("TerminalAsegurada"));
            log.Info("[CmdTerminalAsegurada] Mostrando panel terminal asegurada");

            iu.MostrarPanelTerminalAsegurada();
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
