using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdTerminarVentaEspecialSinMedioPago : ComandoAbstract
    {
        Solicitudes.SolicitudPagarVenta solicitud;

        private void LimpiarVentaFinalizada()
        {
            //
            iu.PanelVentas.LimpiarVentaFinalizada();

            //
            iu.ModalVentaEspecial.LimpiarOperacion();

            //
            Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
            Reactor.Instancia.Procesar(solicitudPanelVenta);
        }

        public CmdTerminarVentaEspecialSinMedioPago(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudPagarVenta;
        }

        public override void Ejecutar()
        {
            Respuesta respuesta = new Respuesta();
            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
            PVentaEspecial ventaEspecial = new PVentaEspecial();
            bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");
            /*if (!bool.TryParse(Parametros.Parametro("pdv.definicion_impuesto_compuesta").Valor, out implementaImpuestoCompuesto))
            {
                implementaImpuestoCompuesto = false;
            }*/

            // Imprimir factura
            string factura = ProcesarPlantilla.Factura(Entorno.Instancia.VentaEspecialSinMedioPago, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario);
            string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";

            log.InfoFormat("[CmdTerminarVentaEspecialSinMedioPago] Venta Finalizada, Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

            string tirillaActual = "";
            iu.PanelVentas.Tirilla.ForEach(x =>
            {
                tirillaActual += String.Format("Codigo: {0}, Descripción: {1} ({4}), Cantidad: {2}, Precio: {3} ", x.Codigo, x.Descripcion, x.Cantidad, x.Subtotal, x.PrecioVentaUnidad);
                tirillaActual += Environment.NewLine;
            });
            log.Info("Items tirilla:" + tirillaActual + Environment.NewLine + "Total: " + Entorno.Instancia.VentaEspecialSinMedioPago.TotalVenta + Environment.NewLine + "Totales Impuestos: " + Entorno.Instancia.VentaEspecialSinMedioPago.ImpuestosIncluidos.Sum(x => x.Value[2]) + Environment.NewLine + "Cant Artículos Vendidos: " + Entorno.Instancia.VentaEspecialSinMedioPago.NumeroDeItemsVenta + Environment.NewLine);

            var tiempoGuardarVentaEspecial = new MetricaTemporizador("TerminarVentaEspecialFinalizada");

            ventaEspecial.GuardarVentaEspecial(Entorno.Instancia.VentaEspecialSinMedioPago, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.VentaEspecial).ToString(), factura, modeloImpresora, implementaImpuestoCompuesto, out respuesta);
            if (respuesta.Valida == false)
            {
                Telemetria.Instancia.AgregaMetrica(tiempoGuardarVentaEspecial.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.VentaEspecialSinMedioPago.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.VentaEspecialSinMedioPago.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.VentaEspecialSinMedioPago.NumeroDeItemsVenta).AgregarPropiedad("Error", respuesta.Mensaje));
            }
            else
            {

                //Log a azure
                Telemetria.Instancia.AgregaMetrica(tiempoGuardarVentaEspecial.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.VentaEspecialSinMedioPago.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.VentaEspecialSinMedioPago.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.VentaEspecialSinMedioPago.NumeroDeItemsVenta));

                // Imprimir
                Entorno.Instancia.Impresora.Imprimir(factura, cortarPapel: true, abrirCajon: false);

                //
                Entorno.Instancia.IdsAcumulados = idsAcumulados;

                //
                respuesta = new Respuesta(false);
                ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                if (respuesta.Valida)
                {
                    Entorno.Instancia.Terminal = terminal;

                    LimpiarVentaFinalizada();
                    log.Info("[CmdTerminarVentaEspecialSinMedioPago] Venta finalizada.");

                    //
                    iu.PanelVentas.VisorCliente.Total = 0;
                    iu.PanelVentas.VisorMensaje = string.Format("Venta Finalizada.");

                    try
                    {
                        Respuesta respuesta2;

                        bool checkFactura = Entorno.Instancia.Terminal.VerificarLimiteNumeracion(out respuesta);
                        bool checkFecha = Entorno.Instancia.Terminal.VerificarFechaAutorizacion(out respuesta2);

                        if (respuesta.Valida && !(respuesta.Mensaje.Equals("")))
                        {
                            iu.PanelVentas.VisorMensaje = string.Format("Venta Finalizada. - " + respuesta.Mensaje);
                        }
                        else if (respuesta2.Valida && !(respuesta2.Mensaje.Equals("")))
                        {
                            iu.PanelVentas.VisorMensaje = string.Format("Venta Finalizada. - " + respuesta2.Mensaje);
                        }

                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("[CmdTerminarVentaEspecialSinMedioPago] {0}", e.Message);
                        Telemetria.Instancia.AgregaMetrica(new Excepcion(e));

                    }

                    //
                    log.InfoFormat("[CmdTerminarVentaEspecialSinMedioPago] --> Transaccion finalizada");

                    iu.PanelOperador.CodigoCliente = "";
                    //
                    iu.PanelVentas.LimpiarOperacion();

                    //
                    Entorno.Instancia.VentaEspecialSinMedioPago.EstaAbierta = false;
                    Entorno.Instancia.VentaEspecialSinMedioPago = null;

                    //
                    if (Entorno.Instancia.Vista.PantallaCliente != null)
                        Entorno.Instancia.Vista.PantallaCliente.MostrarVista(DisplayCliente.DisplayMedia);
                    else
                        Entorno.Instancia.Vista.MostrarDisplayCliente(DisplayCliente.Bienvenida);
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
    }
}
