using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Solicitudes;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdAnularVenta : ComandoAbstract
    {
        public SolicitudCancelarVenta SolicitudVenta { get; set; }

        public CmdAnularVenta(ISolicitud solicitud) : base(solicitud)
        {
            SolicitudVenta = solicitud as SolicitudCancelarVenta;
        }

        public override void Ejecutar()
        {
            // llamar a la persistencia de cancelar transacción
            if (Entorno.Instancia.Venta.EstaAbierta)
            {
                // llamar a la persistencia de cancelar transacción
                Task<MessageResult> resul = null; ;
                if (Config.ViewMode == InternalSettings.ModoConsola)
                {
                    resul = Entorno.Instancia.Vista.PanelVentas.CancelarOperacion("¿Está seguro de cancelar la venta en curso?, [Sí = 1, No = 2]");
                }
                else if (Config.ViewMode == InternalSettings.ModoTouch)
                {
                    Entorno.Instancia.Vista.MensajeUsuario.TextCancelar = "No";
                    Entorno.Instancia.Vista.MensajeUsuario.TextConfirmar = "Sí";
                    resul = Entorno.Instancia.Vista.MensajeUsuario.MostrarMensajeAsync("Cancelar Venta", "¿Está seguro de cancelar la venta en curso?");
                }

                resul.Wait();

                if (resul.Result == MessageResult.Affirmative)
                {
                    PVenta pVenta = new PVenta();
                    Respuesta respuesta = new Respuesta();
                    Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;

                    var tiempoCancelarTransaccion = new MetricaTemporizador("CancelarTransaccion");
                    pVenta.CancelarVenta(Entorno.Instancia.Venta, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.AnularVenta).ToString(), out respuesta);
                    if (respuesta.Valida == false)
                    {
                        Telemetria.Instancia.AgregaMetrica(tiempoCancelarTransaccion.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta).AgregarPropiedad("Error", respuesta.Mensaje));
                    }
                    else
                    {

                        Telemetria.Instancia.AgregaMetrica(tiempoCancelarTransaccion.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta));
                        log.Info("[CmdCancelarTransaccion] --> Transacción cancelada. Factura: " + Entorno.Instancia.Terminal.NumeroUltimaFactura + 1 + " Transaccion: " + Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                        // 
                        Entorno.Instancia.IdsAcumulados = idsAcumulados;
                        Entorno.Instancia.Venta.EstaAbierta = false;

                        // si es teclado touch
                        if (Config.ViewMode == InternalSettings.ModoTouch)
                            iu.PanelVentas.LimpiarOperacion();

                        respuesta = new Respuesta(false);
                        ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                        Entorno.Instancia.Terminal = terminal;

                        //
                        LimpiarTransaccion();
                        iu.PanelOperador.CodigoCliente = "";

                        iu.PanelVentas.LimpiarVentaFinalizada();

                        if (iu.PanelPago != null)
                            iu.PanelPago.LimpiarVentaFinalizada();

                        //
                        iu.PanelVentas.VisorCliente.Total = 0;
                        iu.PanelVentas.VisorMensaje = "Transacción cancelada correctamente";

                        if (Entorno.Instancia.Vista.PantallaCliente != null)
                        {
                            Entorno.Instancia.Vista.LimpiarPantallaCliente();
                            Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);
                        }
                        else
                            Entorno.Instancia.Vista.MostrarDisplayCliente(DisplayCliente.Bienvenida);

                        //
                        iu.PanelVentas.LimpiarVentaFinalizada();

                        //
                        Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
                        Reactor.Instancia.Procesar(solicitudPanelVenta);

                        //
                        if (Config.ViewMode == InternalSettings.ModoTouch)
                            Entorno.Instancia.Vista.MensajeUsuario.OcultarMensaje();
                    }
                }
                else
                {
                    SolicitudVolver solVolver = new SolicitudVolver(Solicitud.Volver);
                    Reactor.Instancia.Procesar(solVolver);

                    //
                    if (Config.ViewMode == InternalSettings.ModoTouch)
                        Entorno.Instancia.Vista.MensajeUsuario.OcultarMensaje();
                }
            }
            else
            {
                LimpiarTransaccion();
            }
        }

        private void LimpiarTransaccion()
        {
            try
            {
                iu.PanelVentas.VisorEntrada = string.Empty;
                
                //
                iu.PanelVentas.LimpiarVentaFinalizada();

                //
                if (Entorno.Instancia.Venta.EstaAbierta)
                    iu.PanelVentas.VisorMensaje = "Transacción cancelada correctamente";

                //
                Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
                Reactor.Instancia.Procesar(solicitudPanelVenta);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
