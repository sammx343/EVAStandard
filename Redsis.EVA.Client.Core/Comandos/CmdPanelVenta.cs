using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Common;
using EvaPOS.Enums;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelVenta : ComandoAbstract
    {
        Solicitudes.SolicitudPanelVenta solicitudPanelVenta;

        public CmdPanelVenta(ISolicitud solicitud) : base(solicitud)
        {
            solicitudPanelVenta = solicitud as Solicitudes.SolicitudPanelVenta;
        }

        public override void Ejecutar()
        {
            //Telemetria.Instancia.AgregaMetrica(new Evento("PanelVentas"));

            log.Info("[CmdPanelVenta] Mostrando panel de ventas");
            iu.MostrarPanelVenta();

            if (iu.PanelVentas != null)
                iu.PanelVentas.VisorFechaActual = DateTime.Now.ToString("dd/MM/yyyy");

            if (Entorno.Venta.EstaAbierta)
            {
                //Mostrar display de ventas.
                if (Reactor.Instancia.EstadoFSMAnterior != EstadosFSM.ConsultarPrecio)
                    iu.MostrarDisplayCliente(Enums.DisplayCliente.DisplayVenta);
            }

            Respuesta respuestaEstadoImpresora = Entorno.Instancia.Impresora.ValidarEstado();
            if (!respuestaEstadoImpresora.Valida)
            {
                //todo: mostrar mensaje de estado de impresora.
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
