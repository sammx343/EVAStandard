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
    class CmdCancelarVentaEspecialSinMedioPago : ComandoAbstract
    {
        #region Properties
        private SolicitudCancelarVentaEspecial Solicitud;
        #endregion

        #region Constructor
        public CmdCancelarVentaEspecialSinMedioPago(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as SolicitudCancelarVentaEspecial;
        }
        #endregion

        #region Methods
        public override void Ejecutar()
        {
            //
            log.Info("[CmdCancelarVentaEspecialSinMedioPago] Cancelando transacción ...");

            // llamar a la persistencia de cancelar transacción
            Task<MessageResult> resul = null; ;
            if (Config.ViewMode == InternalSettings.ModoConsola)
            {
                resul = Entorno.Instancia.Vista.PanelVentas.CancelarOperacion("¿Está seguro de cancelar la venta especial?, [Sí = 1, No = 2]");
            }
            else if (Config.ViewMode == InternalSettings.ModoTouch)
            {
                Entorno.Instancia.Vista.MensajeUsuario.TextCancelar = "No";
                Entorno.Instancia.Vista.MensajeUsuario.TextConfirmar = "Sí";
                resul = Entorno.Instancia.Vista.MensajeUsuario.MostrarMensajeAsync("Cancelar Venta Especial", "¿Está seguro de cancelar la venta especial?");
            }

            //
            resul.Wait();

            if (resul.Result == MessageResult.Affirmative)
            {
                if (Entorno.Instancia.VentaEspecialSinMedioPago.EstaAbierta)
                {

                    // llamar a la persistencia de cancelar transacción

                    PVenta pVenta = new PVenta();
                    Respuesta respuesta = new Respuesta();
                    Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;

                    var tiempoCancelarVentaEspecial = new MetricaTemporizador("CancelarVentaEspecial");
                    pVenta.CancelarVenta(Entorno.Instancia.VentaEspecialSinMedioPago, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.AnularVenta).ToString(), out respuesta);
                    if (respuesta.Valida == false)
                    {
                        Telemetria.Instancia.AgregaMetrica(tiempoCancelarVentaEspecial.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.VentaEspecialSinMedioPago.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.VentaEspecialSinMedioPago.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.VentaEspecialSinMedioPago.NumeroDeItemsVenta).AgregarPropiedad("Error", respuesta.Mensaje));
                    }
                    else
                    {
                        //
                        Telemetria.Instancia.AgregaMetrica(tiempoCancelarVentaEspecial.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.VentaEspecialSinMedioPago.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.VentaEspecialSinMedioPago.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.VentaEspecialSinMedioPago.NumeroDeItemsVenta));
                        log.Info("[CmdCancelarVentaEspecialSinMedioPago] --> Transacción cancelada. Factura: " + Entorno.Instancia.Terminal.NumeroUltimaFactura + 1 + " Transaccion: " + Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                        Entorno.Instancia.IdsAcumulados = idsAcumulados;
                        Entorno.Instancia.VentaEspecialSinMedioPago.EstaAbierta = false;
                        Entorno.Instancia.VentaEspecialSinMedioPago = null;

                        // si es teclado touch
                        if (Config.ViewMode == InternalSettings.ModoTouch)
                            iu.PanelVentas.LimpiarOperacion();

                        respuesta = new Respuesta(false);
                        ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                        if (respuesta.Valida == true)
                        {
                            Entorno.Instancia.Terminal = terminal;

                            //
                            LimpiarTransaccion();
                            //
                            iu.PanelVentas.VisorCliente.Total = 0;
                            iu.PanelVentas.VisorMensaje = "Transacción cancelada correctamente";
                            iu.PanelOperador.CodigoCliente = string.Empty;

                            try
                            {
                                if (Entorno.Instancia.Vista.PantallaCliente != null)
                                    Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);
                                else
                                    Entorno.Instancia.Vista.MostrarDisplayCliente(DisplayCliente.Bienvenida);
                            }
                            catch (Exception ex)
                            {
                                log.ErrorFormat("CmdCancelarVentaEspecialSinMedioPago.Ejecutar [{0}]", ex.ToString());
                            }

                        }
                    }
                }
                else
                {
                    iu.PanelOperador.CodigoCliente = string.Empty;

                    if (Config.ViewMode == InternalSettings.ModoTouch)
                        iu.PanelVentas.LimpiarOperacion();

                    LimpiarTransaccion();

                    Entorno.Instancia.VentaEspecialSinMedioPago = null;

                    try
                    {
                        if (Entorno.Instancia.Vista.PantallaCliente != null)
                        {
                            Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("CmdCancelarVentaEspecialSinMedioPago.Ejecutar [{0}]", ex.ToString());
                    }

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
