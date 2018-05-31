using EvaPOS;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdCancelarTransaccionRecogida : ComandoAbstract
    {
        #region Properties
        private SolicitudCancelarRecogida Solicitud;
        #endregion

        public CmdCancelarTransaccionRecogida(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as SolicitudCancelarRecogida;
        }

        public override void Ejecutar()
        {
            //
            log.Info("[CmdCancelarTransaccionRecogida] Cancelando transacción ...");

            // llamar a la persistencia de cancelar transacción
            Task<MessageResult> resul = null; ;
            if (Config.ViewMode == InternalSettings.ModoConsola)
            {
                resul = Entorno.Instancia.Vista.PanelVentas.CancelarOperacion("¿Está seguro de cancelar la Recogida?, [Sí = 1, No = 2]");
            }
            else if (Config.ViewMode == InternalSettings.ModoTouch)
            {
                Entorno.Instancia.Vista.MensajeUsuario.TextCancelar = "No";
                Entorno.Instancia.Vista.MensajeUsuario.TextConfirmar = "Sí";
                resul = Entorno.Instancia.Vista.MensajeUsuario.MostrarMensajeAsync("Cancelar Recogida", "¿Está seguro de cancelar la Recogida?");
            }

            //
            resul.Wait();

            if (resul.Result == MessageResult.Affirmative)
            {
                if (Config.ViewMode == InternalSettings.ModoTouch)
                    iu.PanelVentas.LimpiarOperacion();

                LimpiarTransaccion();

                if (Entorno.Instancia.Vista.PantallaCliente != null)
                    Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);
            }
            else
            {
                //
                SolicitudPanelVenta solVolver = new Solicitudes.SolicitudPanelVenta(Enums.Solicitud.Volver);
                Reactor.Instancia.Procesar(solVolver);

                //
                if (Config.ViewMode == InternalSettings.ModoTouch)
                    Entorno.Instancia.Vista.MensajeUsuario.OcultarMensaje();
            }
        }

        private void LimpiarTransaccion()
        {
            //
            iu.PanelVentas.LimpiarVentaFinalizada();

            //
            iu.PanelVentas.VisorMensaje = "Transacción cancelada correctamente";

            //
            Solicitudes.SolicitudPanelVenta solicitudPanelVenta = new Solicitudes.SolicitudPanelVenta(Enums.Solicitud.Vender);
            Reactor.Instancia.Procesar(solicitudPanelVenta);

            //
            if (Config.ViewMode == InternalSettings.ModoTouch)
                Entorno.Instancia.Vista.MensajeUsuario.OcultarMensaje();
        }
    }
}
