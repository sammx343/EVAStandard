using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class CmdVerTirilla : ComandoAbstract
    {
        Solicitudes.SolicitudVerTirilla solicitud;

        public CmdVerTirilla(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudVerTirilla;
        }

        public override void Ejecutar()
        {
            if (!Entorno.Instancia.Venta.EstaAbierta && Entorno.Instancia.Devolucion == null)
            {
                log.Warn("[CmdVerTirilla] No hay transacción activa para ver la tirilla");
                iu.PanelVentas.VisorMensaje = "No hay transacción activa";
                Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
                Reactor.Instancia.Procesar(solicitudPanelVenta);
            }
            else
            {
                Telemetria.Instancia.AgregaMetrica(new Evento("PanelTirilla"));

                log.Info("[CmdVerTirilla] Mostrando panel de tirilla");

                iu.MostrarTirilla();
            }
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
