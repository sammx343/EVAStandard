using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Persistencia;
//using Redsis.EVA.Client.Dispositivos;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Solicitudes;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdTerminarDevolucion : ComandoAbstract
    {
        Solicitudes.SolicitudPagarVenta solicitud;

        private void LimpiarVentaFinalizada()
        {
            //
            iu.PanelVentas.LimpiarVentaFinalizada();

            //
            Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
            Reactor.Instancia.Procesar(solicitudPanelVenta);
        }

        public CmdTerminarDevolucion(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudPagarVenta;
        }

        public override void Ejecutar()
        {
            Respuesta respuesta = new Respuesta();
            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
            PDevolucion devolucion = new PDevolucion();
            bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");
            /*if (!bool.TryParse(Parametros.Parametro("pdv.definicion_impuesto_compuesta").Valor, out implementaImpuestoCompuesto))
            {
                implementaImpuestoCompuesto = false;
            }*/

            // Imprimir factura
            string factura = ProcesarPlantilla.Factura(Entorno.Instancia.Devolucion, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario);
            string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";

            //
            log.InfoFormat("[CmdPagarVenta] Devolucion Finalizada, Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

            string tirillaActual = "";
            iu.PanelVentas.Tirilla.ForEach(x =>
            {
                tirillaActual += String.Format("Codigo: {0}, Descripción: {1} ({4}), Cantidad: {2}, Precio: {3} ", x.Codigo, x.Descripcion, x.Cantidad, x.Subtotal, x.PrecioVentaUnidad);
                tirillaActual += Environment.NewLine;
            });
            log.Info("Items tirilla:" + tirillaActual + Environment.NewLine + "Total: " + Entorno.Instancia.Devolucion.TotalVenta + Environment.NewLine + "Totales Impuestos: " + Entorno.Instancia.Devolucion.ImpuestosIncluidos.Sum(x => x.Value[2]) + Environment.NewLine + "Cant Artículos Vendidos: " + Entorno.Instancia.Devolucion.NumeroDeItemsVenta + Environment.NewLine);

            var tiempoGuardarDevolucion = new MetricaTemporizador("TerminarDevolucionFinalizada");

            devolucion.GuardarDevolucion(Entorno.Instancia.Devolucion, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.Devolucion).ToString(), factura, modeloImpresora, implementaImpuestoCompuesto, out respuesta);
            if (respuesta.Valida == false)
            {
                this.iu.PanelVentas.VisorMensaje = "No se pudo procesar la devolución, intente nuevamente.";
                Telemetria.Instancia.AgregaMetrica(tiempoGuardarDevolucion.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Devolucion.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Devolucion.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Devolucion.NumeroDeItemsVenta).AgregarPropiedad("Error:", respuesta.Mensaje));

                //
                SolicitudVolver volver = new SolicitudVolver(Solicitud.Volver);
                Reactor.Instancia.Procesar(volver);

                //
                LimpiarVentaFinalizada();
            }
            else
            {

                //Log a azure
                Telemetria.Instancia.AgregaMetrica(tiempoGuardarDevolucion.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Devolucion.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Devolucion.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Devolucion.NumeroDeItemsVenta));

                // Imprimir
                Entorno.Instancia.Impresora.Imprimir(factura, cortarPapel: true, abrirCajon: true);

                //
                Entorno.Instancia.IdsAcumulados = idsAcumulados;

                //
                respuesta = new Respuesta(false);
                ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                Entorno.Instancia.Terminal = terminal;

                LimpiarVentaFinalizada();
                log.Debug("[CmdTerminarDevolucion] Devolucion finalizada.");

                //
                decimal valorCambio = Entorno.Instancia.Devolucion.TotalVenta * -1;
                iu.PanelVentas.VisorCliente.Total = 0;
                iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0}", valorCambio.ToCustomCurrencyFormat());

                try
                {
                    Respuesta respuesta2;

                    bool checkFactura = Entorno.Instancia.Terminal.VerificarLimiteNumeracion(out respuesta);
                    bool checkFecha = Entorno.Instancia.Terminal.VerificarFechaAutorizacion(out respuesta2);

                    if (respuesta.Valida && !(respuesta.Mensaje.Equals("")))
                    {
                        iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0} - " + respuesta.Mensaje, valorCambio.ToCustomCurrencyFormat());
                    }
                    else if (respuesta2.Valida && !(respuesta2.Mensaje.Equals("")))
                    {
                        iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0} - " + respuesta2.Mensaje, valorCambio.ToCustomCurrencyFormat());
                    }
                    
                }
                catch (Exception e)
                {
                    log.ErrorFormat("[CmdterminarDevolucion] {0}", e.Message);
                    Telemetria.Instancia.AgregaMetrica(new Excepcion(e));

                }

                //
                log.InfoFormat("[CmdTerminarDevolucion] --> Transaccion finalizada");

                //
                iu.PanelOperador.CodigoCliente = "";

                // Mostrar fin devolución en pantalla cliente.
                iu.MostrarDisplayCliente(DisplayCliente.FinVenta);

                //
                Entorno.Instancia.Devolucion.EstaAbierta = false;
                Entorno.Instancia.Devolucion = null;

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
