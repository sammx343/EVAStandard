using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Solicitudes;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdPrestamos : ComandoAbstract
    {

        #region Properties
        private SolicitudPanelPrestamos Solicitud;
        private string tipoPrestamo;
        #endregion

        #region Constructor
        public CmdPrestamos(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as SolicitudPanelPrestamos;
            this.tipoPrestamo = Solicitud.TipoPrestamo;
        }
        #endregion

        public override void Ejecutar()
        {
            Telemetria.Instancia.AgregaMetrica(new Evento("PanelPrestamos"));

            log.Info("[CmdPrestamos] Mostrando panel de prestamos");

            if (Config.ViewMode == InternalSettings.ModoTouch)
            {
                log.Debug("[CmdPrestamos] Estado Actual: " + Reactor.Instancia.EstadoFSMActual.GetEnumName());
                iu.PanelVentas.VisorMensaje = "Ingrese Valor del prestamo.";
            }
            else
            {
                iu.MostrarPanelPrestamos();
                iu.PanelPrestamos.TipoPrestamo = tipoPrestamo ?? "Ingreso de Base";
                iu.PanelPrestamos.VisorOperador = "Ingrese Valor";

                iu.PanelPrestamos.VisorEntrada = string.Empty;
                iu.PanelPrestamos.VisorMensaje = string.Empty;
                //iu.PanelOperador.MensajeOperador = string.Empty;

                //
                List<decimal> listPrestamos = new List<decimal>();
                Entorno.Instancia.Prestamo = new Entidades.EPrestamo();
                Entorno.Instancia.Prestamo.ListPrestamos = listPrestamos;
            }

            try
            {
                if (Entorno.Instancia.Impresora != null)
                    Entorno.Instancia.Impresora.AbrirCajon();
            }
            catch (Exception ex)
            {
                Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "No se pudo abrir el cajón monedero.";
                log.Error($"[CmdPrestamos.Ejecutar] {ex.Message}");
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
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