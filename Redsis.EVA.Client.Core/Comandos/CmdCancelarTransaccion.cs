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
    public class CmdCancelarTransaccion : ComandoAbstract
    {
        #region Properties
        private SolicitudCancelarTransaccion Solicitud;
        public string ValorEntrada { get; set; }
        #endregion

        #region Constructor
        public CmdCancelarTransaccion(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as SolicitudCancelarTransaccion;
            ValorEntrada = Solicitud.ValorEntrada;
        }
        #endregion

        #region Methods
        public override void Ejecutar()
        {
            //
            log.Info("[CmdCancelarTransaccion] Cancelando transacción ...");

            //
            bool cancTransaccion = false;

            if (!string.IsNullOrEmpty(ValorEntrada))
            {
                cancTransaccion = ValorEntrada == "1";
            }
            else
            {
                if (Config.ViewMode == InternalSettings.ModoTouch)
                    cancTransaccion = iu.PanelVentas.CancelarTransaccion();
            }

            if (cancTransaccion)
            {
                // llamar a la persistencia de cancelar transacción

                PVenta pVenta = new PVenta();
                Respuesta respuesta = new Respuesta();
                Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;

                var tiempoCancelarTransaccion = new MetricaTemporizador("CancelarTransaccion");
                pVenta.CancelarVenta(Entorno.Instancia.Venta, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.AnularVenta).ToString(), out respuesta);
                if (respuesta.Valida == false)
                {
                    Telemetria.Instancia.AgregaMetrica(tiempoCancelarTransaccion.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta).AgregarPropiedad("Error",respuesta.Mensaje));
                }
                else
                {

                    Telemetria.Instancia.AgregaMetrica(tiempoCancelarTransaccion.Para().AgregarPropiedad("Exitoso",true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta));
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

                    if(iu.PanelPago!=null)
                        iu.PanelPago.LimpiarVentaFinalizada();

                    //
                    iu.PanelVentas.VisorCliente.Total = 0;
                    iu.PanelVentas.VisorMensaje = "Transacción cancelada correctamente";

                    if (Entorno.Instancia.Vista.PantallaCliente != null)
                    {
                        Entorno.Instancia.Vista.LimpiarPantallaCliente();
                        Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);
                    }

                }
            }
            else
            {
                //
                if (Config.ViewMode == InternalSettings.ModoTouch)
                {
                    Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
                    Reactor.Instancia.Procesar(solicitudPanelVenta);
                }
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
            iu.PanelVentas.VisorMensaje = "Transacción cancelada correctamente";

            //
            Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
            Reactor.Instancia.Procesar(solicitudPanelVenta);
        }
        #endregion
    }
}
