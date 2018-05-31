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
    public class CmdCancelarTransaccionAjuste : ComandoAbstract
    {
        #region Properties
        private SolicitudCancelarAjuste Solicitud;
        #endregion

        #region Constructor
        public CmdCancelarTransaccionAjuste(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as SolicitudCancelarAjuste;
        }
        #endregion

        #region Methods
        public override void Ejecutar()
        {
            //
            log.Info("[CmdCancelarDevolucion.Ejecutar] Cancelando transacción ...");

            // llamar a la persistencia de cancelar transacción
            Task<MessageResult> resul = null; ;
            if (Config.ViewMode == InternalSettings.ModoConsola)
            {
                resul = Entorno.Instancia.Vista.PanelVentas.CancelarOperacion("¿Está seguro de cancelar el ajuste?, [Sí = 1, No = 2]");
            }
            else if (Config.ViewMode == InternalSettings.ModoTouch)
            {
                Entorno.Instancia.Vista.MensajeUsuario.TextCancelar = "No";
                Entorno.Instancia.Vista.MensajeUsuario.TextConfirmar = "Sí";
                resul = Entorno.Instancia.Vista.MensajeUsuario.MostrarMensajeAsync("Cancelar Ajuste", "¿Está seguro de cancelar el ajuste?");
            }

            //
            resul.Wait();

            //
            if (resul.Result == MessageResult.Affirmative)
            {
                if (Entorno.Instancia.Ajuste.EstaAbierta)
                {
                    // llamar a la persistencia de cancelar transacción

                    PVenta pVenta = new PVenta();
                    Respuesta respuesta = new Respuesta();
                    Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;

                    var tiempoCancelarAjuste = new MetricaTemporizador("CancelarAjuste");
                    pVenta.CancelarVenta(Entorno.Instancia.Ajuste, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.AnularVenta).ToString(), out respuesta);
                    if (respuesta.Valida == false)
                    {
                        Telemetria.Instancia.AgregaMetrica(tiempoCancelarAjuste.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Ajuste.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Ajuste.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Ajuste.NumeroDeItemsVenta).AgregarPropiedad("Error", respuesta.Mensaje));
                    }
                    else
                    {

                        //
                        Telemetria.Instancia.AgregaMetrica(tiempoCancelarAjuste.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Ajuste.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Ajuste.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Ajuste.NumeroDeItemsVenta));
                        log.Info("[CmdCancelarTransaccionAjuste] --> Transacción cancelada. Factura: " + Entorno.Instancia.Terminal.NumeroUltimaFactura + 1 + " Transaccion: " + Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                        Entorno.Instancia.IdsAcumulados = idsAcumulados;
                        Entorno.Instancia.Ajuste.EstaAbierta = false;
                        Entorno.Instancia.Ajuste = null;

                        // si es teclado touch
                        if (Config.ViewMode == InternalSettings.ModoTouch)
                            iu.PanelVentas.LimpiarOperacion();

                        respuesta = new Respuesta(false);
                        ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                        Entorno.Instancia.Terminal = terminal;

                        //
                        LimpiarTransaccion();
                        //
                        iu.PanelVentas.VisorCliente.Total = 0;
                        iu.PanelVentas.VisorMensaje = "Transacción cancelada correctamente";

                        if (Entorno.Instancia.Vista.PantallaCliente != null)
                            Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);

                        //
                        Entorno.Instancia.Vista.MostrarDisplayCliente(DisplayCliente.Bienvenida);
                    }
                }
                else
                {
                    if (Config.ViewMode == InternalSettings.ModoTouch)
                        iu.PanelVentas.LimpiarOperacion();

                    LimpiarTransaccion();

                    Entorno.Instancia.Ajuste = null;

                    if (Entorno.Instancia.Vista.PantallaCliente != null)
                        Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);

                    //
                    Entorno.Instancia.Vista.MostrarDisplayCliente(DisplayCliente.Bienvenida);
                }
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
        #endregion
    }
}
