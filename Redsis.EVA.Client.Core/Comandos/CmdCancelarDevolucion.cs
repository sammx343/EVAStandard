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
    public class CmdCancelarDevolucion : ComandoAbstract
    {
        #region Properties

        private SolicitudCancelarDevolucion Solicitud;

        #endregion

        #region Constructor
        public CmdCancelarDevolucion(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as SolicitudCancelarDevolucion;
        }
        #endregion

        #region Methods

        public override void Ejecutar()
        {
            try
            {
                if (Entorno.Instancia.Devolucion != null)
                {
                    //
                    log.Info("[CmdCancelarDevolucion] Cancelando devolución...");

                    if (Entorno.Instancia.Devolucion.EstaAbierta)
                    {
                        // llamar a la persistencia de cancelar transacción
                        Task<MessageResult> resul = null; ;
                        if (Config.ViewMode == InternalSettings.ModoConsola)
                        {
                            resul = Entorno.Instancia.Vista.PanelVentas.CancelarOperacion("¿Está seguro de cancelar la devolución?, [Sí = 1, No = 2]");
                        }
                        else if (Config.ViewMode == InternalSettings.ModoTouch)
                        {
                            Entorno.Instancia.Vista.MensajeUsuario.TextCancelar = "No";
                            Entorno.Instancia.Vista.MensajeUsuario.TextConfirmar = "Sí";
                            resul = Entorno.Instancia.Vista.MensajeUsuario.MostrarMensajeAsync("Cancelar Devolución", "¿Está seguro de cancelar la devolución?");
                        }

                        //
                        resul.Wait();

                        if (resul.Result == MessageResult.Affirmative)
                        {
                            PVenta pVenta = new PVenta();
                            Respuesta respuesta = new Respuesta();
                            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;

                            var tiempoCancelarDevolucion = new MetricaTemporizador("CancelarDevolucion");
                            pVenta.CancelarVenta(Entorno.Instancia.Devolucion, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.AnularVenta).ToString(), out respuesta);
                            if (respuesta.Valida == false)
                            {
                                Telemetria.Instancia.AgregaMetrica(tiempoCancelarDevolucion.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Devolucion.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Devolucion.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Devolucion.NumeroDeItemsVenta).AgregarPropiedad("Error", respuesta.Mensaje));
                            }
                            else
                            {
                                //
                                Telemetria.Instancia.AgregaMetrica(tiempoCancelarDevolucion.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Devolucion.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Devolucion.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Devolucion.NumeroDeItemsVenta));
                                log.Info("[CmdCancelarDevolucion] --> Transacción cancelada. Factura: " + Entorno.Instancia.Terminal.NumeroUltimaFactura + 1 + " Transaccion: " + Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                                Entorno.Instancia.IdsAcumulados = idsAcumulados;
                                Entorno.Instancia.Devolucion.EstaAbierta = false;
                                Entorno.Instancia.Devolucion = null;

                                // si es teclado touch
                                if (Config.ViewMode == InternalSettings.ModoTouch)
                                    iu.PanelVentas.LimpiarOperacion();

                                respuesta = new Respuesta(false);
                                ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                                Entorno.Instancia.Terminal = terminal;

                                //
                                LimpiarTransaccion();
                                iu.PanelOperador.CodigoCliente = "";

                                //
                                iu.PanelVentas.VisorCliente.Total = 0;
                                iu.PanelVentas.VisorMensaje = "Transacción cancelada correctamente";
                                Entorno.Instancia.Vista.LimpiarPantallaCliente();

                                if (Entorno.Instancia.Vista.PantallaCliente != null)
                                    Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);
                                else
                                    Entorno.Instancia.Vista.MostrarDisplayCliente(DisplayCliente.Bienvenida);
                            }
                        }
                        else
                        {
                            //
                            Solicitudes.SolicitudVolver solVolver = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
                            Reactor.Instancia.Procesar(solVolver);

                            //
                            if (Config.ViewMode == InternalSettings.ModoTouch)
                                Entorno.Instancia.Vista.MensajeUsuario.OcultarMensaje();
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(100);

                        LimpiarTransaccion();

                        if (Entorno.Instancia.Vista.PantallaCliente != null)
                            Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);

                        //
                        Entorno.Instancia.Devolucion.EstaAbierta = false;
                        Entorno.Instancia.Devolucion = null;

                        //
                        Entorno.Instancia.Vista.MostrarDisplayCliente(DisplayCliente.Bienvenida);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("[CmdCancelarDevolucion.Ejecutar] " + ex.Message);
            }
        }

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }

        private void LimpiarTransaccion()
        {
            //
            iu.PanelVentas.LimpiarVentaFinalizada();

            //
            Solicitudes.SolicitudVolver solVolver = new Solicitudes.SolicitudVolver(Enums.Solicitud.Vender);
            Reactor.Instancia.Procesar(solVolver);

            //
            if (Config.ViewMode == InternalSettings.ModoTouch)
            {
                if (Entorno.Instancia.Devolucion == null)
                    Entorno.Instancia.Vista.MensajeUsuario.OcultarMensaje();
            }
        }
        #endregion
    }
}
