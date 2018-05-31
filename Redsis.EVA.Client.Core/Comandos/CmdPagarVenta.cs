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
using Newtonsoft.Json;
using System.Diagnostics;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Solicitudes;
using EvaPOS;
using EvaPOS.Enums;

namespace Redsis.EVA.Client.Core.Comandos
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class CmdPagarVenta : ComandoAbstract
    {
        Solicitudes.SolicitudPagarVenta solicitud;

        public decimal ValorPago { get; private set; }

        public CmdPagarVenta(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudPagarVenta;
            if (this.solicitud.Pago != null)
            {
                ValorPago = this.solicitud.Pago.Valor;
            }
        }

        private void LimpiarVentaFinalizada()
        {
            //
            iu.PanelPago.VisorMensaje = "Transacción terminada";
            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = string.Empty;

            //
            iu.PanelVentas.LimpiarVentaFinalizada();

            iu.PanelPago.LimpiarVentaFinalizada();

            if (iu.PanelPagoManual != null)
                iu.PanelPagoManual.LimpiarPagoFinalizado();

            //
            Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
            Reactor.Instancia.Procesar(solicitudPanelVenta);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Ejecutar()
        {
            log.Info("[CmdPagarVenta] Pago Ingresado: " + solicitud.ValorEntrada);

            bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");
            bool obligaIngresarValor = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.pago.obliga_ingresar_valor");

            //copia de seguridad de venta por si algo falla.
            string jsonCopiaVenta = JsonConvert.SerializeObject(Entorno.Instancia.Venta, Formatting.Indented);

            //string jsonCopiaImpuestos = JsonConvert.SerializeObject(Entorno.Instancia.Venta.ImpuestosIncluidos, Formatting.Indented);
            var copiaImpuestosIncluidos = Entorno.Instancia.Venta.ImpuestosIncluidos;

            try
            {
                #region valida entrada de pago

                //obtiene el valor ingresado para pagar la venta.
                decimal pago = -1;
                string entrada = solicitud.ValorEntrada;

                if (string.IsNullOrEmpty(entrada))
                {
                    if (this.solicitud.Pago != null)
                    {
                        pago = this.solicitud.Pago.Valor;
                        if (pago == 0)
                        {
                            pago = Entorno.Instancia.Vista.PanelPago.VisorCliente.Total;
                            this.solicitud.Pago.Valor = pago;
                        }
                    }
                    else
                    {
                        if (obligaIngresarValor)
                        {
                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Debe ingresar un valor a pagar.";
                            log.Warn("Se obliga a ingresar un valor a pagar.");

                            //
                            Solicitudes.SolicitudPanelPago solVolver = new Solicitudes.SolicitudPanelPago(Enums.Solicitud.Pagar, "Debe ingresar un valor a pagar.");
                            Reactor.Instancia.Procesar(solVolver);
                            return;
                        }
                        else
                        {
                            //Toma el total de la venta.
                            pago = Entorno.Instancia.Vista.PanelPago.VisorCliente.Total;
                        }
                    }
                }
                else
                {
                    if (entrada == "VentaPagada")
                    {
                        //Se calcula el total ($) de artículos cancelados + el total de pagos realizados.
                        log.Info("La venta ha sido pagada, se finaliza de la venta.");
                        decimal totalPagos = (Entorno.Instancia.Venta.Pagos.Sum(p => p.Valor));
                        pago = totalPagos;


                    }
                    else if (!decimal.TryParse(entrada, out pago))
                    {
                        log.Warn("El valor ingresado no es válido");
                        Entorno.Instancia.Vista.PanelPago.VisorMensaje = "El valor ingresado no es válido";
                    }
                }

                if (pago <= 0)
                {
                    log.WarnFormat("Monto no válido [{0}]", pago);
                    Entorno.Instancia.Vista.PanelPago.VisorMensaje = "Monto no válido";
                    //
                    SolicitudPanelPago volver = new SolicitudPanelPago(Enums.Solicitud.Pagar, "Monto no válido");
                    Reactor.Instancia.Procesar(volver);
                    return;
                }
                else
                {
                    this.ValorPago = pago;
                }

                #endregion


                //valida el total de la venta.

                //pago total de la venta
                Respuesta respuesta = new Respuesta();
                if (this.ValorPago == Entorno.Instancia.Venta.PorPagar)
                {
                    //
                    EMedioPago medioPago = null;
                    EPago ePago = null;
                    if (this.solicitud.Pago != null)
                    {
                        ePago = this.solicitud.Pago;
                        medioPago = ePago.MedioPago;
                    }
                    else
                    {
                        if (this.solicitud.TipoSolicitud == Solicitud.PagoEfectivo)
                        {
                            if (entrada != "VentaPagada")
                            {
                                var medioPagoEfectivo = Entorno.Instancia.MediosPago.Where(m => m.MedioPago == MediosPago.Efectivo);
                                if (!medioPagoEfectivo.IsNullOrEmptyList())
                                {
                                    medioPago = new PMediosPago().GetAllMediosPago().MedioPago(medioPagoEfectivo.FirstOrDefault().CodigoMedioPago);
                                    ePago = new EPago(medioPago, this.ValorPago);
                                }
                                else
                                {
                                    throw new Exception($"No se encontró medio de pago {MediosPago.Efectivo} configurado en entorno");
                                }
                            }
                        }
                        else
                        {
                            log.Error($"La solicitud contiene un pago nulo o vacío y el medio de pago no efectivo. Solicitud: {this.solicitud}");
                            throw new Exception($"No se encontró medio de pago {MediosPago.Efectivo} configurado en entorno");
                        }
                    }

                    if (entrada != "VentaPagada")
                    {
                        Entorno.Instancia.Venta.AgregarPago(medioPago, ePago, out respuesta);
                        if (respuesta.Valida)
                        {
                            Telemetria.Instancia.AgregaMetrica(new Evento("AgregarPago").AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("Valor", (ePago.Valor)));
                            log.InfoFormat("[CmdPagarVenta] Pago Agregado: {0}, Transaccion: {1}, Factura {2}", ePago.Valor, (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));
                        }
                        else
                        {
                            log.WarnFormat("[CmdPagarVenta.Ejecutar] {0}", respuesta.Mensaje);
                            return;
                        }
                    }
                    else
                    {
                        Entorno.Instancia.Venta.PorPagar = 0;
                    }

                    //Validar saldo pendiente por pagar.
                    if (Entorno.Instancia.Venta.PorPagar == 0)
                    {
                        Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
                        string factura = ProcesarPlantilla.Factura(Entorno.Instancia.Venta, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario);
                        string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";
                        PVenta venta = new PVenta();

                        //Log a azure

                        string tirillaActual = "";
                        iu.PanelVentas.Tirilla.ForEach(x =>
                        {
                            tirillaActual += Environment.NewLine;
                            tirillaActual += String.Format("Código: {0}, Descripción: {1} ({4}), Cantidad: {2}, Precio: {3} ", x.Codigo, x.Descripcion, x.Cantidad, x.Subtotal, x.PrecioVentaUnidad);
                        });
                        log.Info("Items tirilla:" + tirillaActual + Environment.NewLine + "Total: " + Entorno.Instancia.Venta.TotalVenta + Environment.NewLine + "Totales Impuestos: " + Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2]) + Environment.NewLine + "Cant Artículos Vendidos: " + Entorno.Instancia.Venta.NumeroDeItemsVenta + Environment.NewLine);

                        //
                        log.InfoFormat("[CmdPagarVenta] Venta Finalizada, Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                        var tiempoGuardarVenta = new MetricaTemporizador("TerminarVentaFinalizada");

                        venta.GuardarVenta(Entorno.Instancia.Venta, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.Venta).ToString(), factura, modeloImpresora, implementaImpuestoCompuesto, out respuesta);
                        if (respuesta.Valida == false)
                        {
                            Telemetria.Instancia.AgregaMetrica(tiempoGuardarVenta.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta).AgregarPropiedad("Error", respuesta.Mensaje));
                            throw new Exception(respuesta.Mensaje);
                        }
                        else
                        {

                            Telemetria.Instancia.AgregaMetrica(tiempoGuardarVenta.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta));
                            Entorno.Instancia.IdsAcumulados = idsAcumulados;
                            //
                            Entorno.Instancia.Venta.EstaAbierta = false;


                            //
                            respuesta = new Respuesta(false);
                            ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                            if (respuesta.Valida)
                            {

                                Entorno.Instancia.Terminal = terminal;

                                string mensaje = string.Empty;

                                //Valida si debe abrir cajón monedero.
                                bool abreCajon = false;
                                var mediosPagoEntorno = new PMediosPago().GetAllMediosPago().ListaMediosPago;
                                var mediosPagoVenta = (from m in mediosPagoEntorno
                                                       join mp in Entorno.Instancia.Venta.Pagos on m.Codigo.ToLower() equals mp.MedioPago.Codigo.ToLower()
                                                       select m);


                                //Verifica si hay algún medio de pago configurado para abrir cajón entre los pagos realizados.
                                var mediosPagoAbreCajon = mediosPagoVenta.Where(m => m.AbreCajon);
                                if (!mediosPagoAbreCajon.IsNullOrEmptyList())
                                {
                                    abreCajon = true;
                                }
                                else
                                {
                                    if (medioPago != null)
                                        abreCajon = medioPago.AbreCajon;
                                }

                                //Imprimir
                                Respuesta resImpresion = Entorno.Instancia.Impresora.Imprimir(factura, true, abreCajon);

                                //
                                LimpiarVentaFinalizada();

                                //
                                if (!resImpresion.Valida)
                                {
                                    Entorno.Vista.PanelOperador.MensajeOperador = resImpresion.Mensaje;
                                }


                                //
                                log.Info("[CmdPagarVenta.Ejecutar] --> Transaccion finalizada.");

                                // 
                                iu.PanelOperador.CodigoCliente = "";

                                //
                                decimal valorCambio = Entorno.Instancia.Venta.PorPagar * -1;
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

                                    decimal valorLimite = Entorno.Instancia.Parametros.ObtenerValorParametro<Decimal>("pdv.aviso_monto_max_en_caja");
                                    decimal valorCaja = venta.DineroEnCaja(Entorno.Instancia.Terminal.Codigo, Entorno.Instancia.Usuario.IdUsuario, out respuesta);
                                    log.InfoFormat("[CmdPagarVenta.Ejecutar] --> Valor en caja : {0}, Valor máximo: {1}", valorCaja.ToCustomCurrencyFormat(), valorLimite.ToCustomCurrencyFormat());
                                    if (valorCaja > valorLimite)
                                    {
                                        iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0} - Tope máximo en caja excedido", valorCambio.ToCustomCurrencyFormat());
                                    }
                                }
                                catch (Exception e)
                                {
                                    log.ErrorFormat("[CmdPagarVenta] {0}", e.Message);
                                    Telemetria.Instancia.AgregaMetrica(new Excepcion(e));

                                }

                                //
                                iu.MostrarPanelVenta();
                            }
                        }

                    }


                }
                else if (this.ValorPago < Entorno.Instancia.Venta.PorPagar)
                {
                    //
                    EMedioPago medioPago = null;
                    EPago ePago = null;
                    if (this.solicitud.Pago != null)
                    {
                        ePago = this.solicitud.Pago;
                        medioPago = ePago.MedioPago;
                    }
                    else
                    {
                        medioPago = new PMediosPago().GetAllMediosPago().MedioPago("1");
                        ePago = new EPago(medioPago, this.ValorPago);
                    }
                    Entorno.Instancia.Venta.AgregarPago(medioPago, ePago, out respuesta);
                    if (!respuesta.Valida)
                    {
                        log.WarnFormat("[CmdPagarVenta.Ejecutar.277] {0}", respuesta.Mensaje);
                        return;
                    }
                    else
                    {
                        // agregar a lista de medio de pago
                        Entorno.Instancia.Vista.PanelPago.AgregarMedioPagoUI(new DTOs.DItemMedioPago { CodigoMedioPago = ePago.MedioPago.Codigo, NombreMedioPago = ePago.MedioPago.Tipo, ValorMedioPago = ePago.Valor });

                        //
                        iu.PanelVentas.VisorCliente.Total = Entorno.Instancia.Venta.PorPagar;
                        iu.PanelPago.VisorCliente.Total = Entorno.Instancia.Venta.PorPagar;
                        iu.PanelPago.VisorEntrada = string.Empty;
                        iu.PanelPago.VisorMensaje = "";

                        //
                        log.InfoFormat("[CmdPagarVenta] Medio de pago agregado. Valor: [{0}]", this.ValorPago.ToCustomCurrencyFormat());

                        if (Reactor.Instancia.EstadoFSMActual != EstadosFSM.Pago)
                        {
                            Solicitudes.SolicitudPanelPago solicitud = new Solicitudes.SolicitudPanelPago(Solicitud.Pagar);
                            Reactor.Instancia.Procesar(solicitud);
                        }

                        //Valida si no está el panel de pago activo.
                        if (!(iu.PanelActivo is IPanelPago))
                        {
                            if (iu.PanelPagoManual != null)
                                iu.PanelPagoManual.LimpiarPagoFinalizado();

                            if (Reactor.Instancia.EstadoFSMActual != EstadosFSM.Pago)
                            {
                                Solicitudes.SolicitudPanelPago solicitud = new Solicitudes.SolicitudPanelPago(Solicitud.Pagar);
                                Reactor.Instancia.Procesar(solicitud);
                            }
                        }
                    }
                }
                else if (this.ValorPago > Entorno.Instancia.Venta.PorPagar)
                {
                    //
                    EMedioPago medioPago = null;
                    EPago ePago = null;
                    if (this.solicitud.Pago != null)
                    {
                        ePago = this.solicitud.Pago;
                        medioPago = ePago.MedioPago;
                    }
                    else
                    {
                        medioPago = new PMediosPago().GetAllMediosPago().MedioPago("1");
                        ePago = new EPago(medioPago, this.ValorPago);
                    }
                    Entorno.Instancia.Venta.AgregarPago(medioPago, ePago, out respuesta);
                    if (!respuesta.Valida)
                    {
                        log.WarnFormat("[CmdPagarVenta.Ejecutar.329] {0}", respuesta.Mensaje);
                        return;
                    }
                    else
                    {
                        //Validar saldo pendiente por pagar.
                        if (Entorno.Instancia.Venta.PorPagar < 0)
                        {
                            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
                            PVenta venta = new PVenta();
                            respuesta = new Respuesta();

                            //
                            string factura = ProcesarPlantilla.Factura(Entorno.Instancia.Venta, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario);
                            string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";

                            //
                            log.InfoFormat("[CmdPagarVenta] Venta Finalizada, Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                            string tirillaActual = "";
                            iu.PanelVentas.Tirilla.ForEach(x =>
                            {
                                tirillaActual += Environment.NewLine;
                                tirillaActual += String.Format("Codigo: {0}, Descripción: {1} ({4}), Cantidad: {2}, Precio: {3} ", x.Codigo, x.Descripcion, x.Cantidad, x.Subtotal, x.PrecioVentaUnidad);
                            });
                            log.Info("Items tirilla:" + tirillaActual + Environment.NewLine + "Total: " + Entorno.Instancia.Venta.TotalVenta + Environment.NewLine + "Totales Impuestos: " + Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2]) + Environment.NewLine + "Cant Artículos Vendidos: " + Entorno.Instancia.Venta.NumeroDeItemsVenta + Environment.NewLine);

                            var tiempoGuardarVenta = new MetricaTemporizador("TerminarVentaFinalizada");

                            venta.GuardarVenta(Entorno.Instancia.Venta, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.Venta).ToString(), factura, modeloImpresora, implementaImpuestoCompuesto, out respuesta);
                            if (respuesta.Valida == false)
                            {
                                Telemetria.Instancia.AgregaMetrica(tiempoGuardarVenta.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta));
                                throw new Exception(respuesta.Mensaje);
                            }

                            //Log a azure
                            Telemetria.Instancia.AgregaMetrica(tiempoGuardarVenta.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta));

                            //
                            decimal valorCambio = Entorno.Instancia.Venta.PorPagar * -1;

                            // 
                            Entorno.Instancia.Impresora.Imprimir(factura, true, medioPago.AbreCajon);

                            //
                            Entorno.Instancia.Venta.EstaAbierta = false;
                            Entorno.Instancia.IdsAcumulados = idsAcumulados;

                            //
                            respuesta = new Respuesta(false);
                            ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                            Entorno.Instancia.Terminal = terminal;

                            // 
                            LimpiarVentaFinalizada();

                            // 
                            iu.PanelOperador.CodigoCliente = "";

                            //
                            iu.PanelVentas.VisorCliente.Total = 0;
                            iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0}", valorCambio.ToCustomCurrencyFormat());
                            try
                            {
                                decimal valorLimite = Entorno.Instancia.Parametros.ObtenerValorParametro<Decimal>("pdv.aviso_monto_max_en_caja");
                                decimal valorCaja = venta.DineroEnCaja(Entorno.Instancia.Terminal.Codigo, Entorno.Instancia.Usuario.IdUsuario, out respuesta);
                                log.InfoFormat("[CmdPagarVenta.Ejecutar] --> Valor en caja : {0}, Valor máximo: {1}", valorCaja.ToCustomCurrencyFormat(), valorLimite.ToCustomCurrencyFormat());
                                if (valorCaja > valorLimite)
                                {
                                    iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0} - Tope máximo en caja excedido", valorCambio.ToCustomCurrencyFormat());
                                }
                            }
                            catch (Exception e)
                            {
                                log.ErrorFormat("[CmdPagarVenta] {0}", e.Message);
                                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));

                            }

                            //
                            log.InfoFormat("[CmdPagarVenta] --> Transaccion finalizada, cambio {0}", valorCambio.ToCustomCurrencyFormat());

                            iu.MostrarPanelVenta();
                        }
                    }
                }
                else if (this.ValorPago == -1)
                {
                    Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
                    PVenta venta = new PVenta();
                    respuesta = new Respuesta();
                    //
                    string factura = ProcesarPlantilla.Factura(Entorno.Instancia.Venta, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario);
                    string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";


                    //
                    log.InfoFormat("[CmdPagarVenta] Venta Finalizada, Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                    string tirillaActual = "";
                    iu.PanelVentas.Tirilla.ForEach(x =>
                    {
                        tirillaActual += Environment.NewLine;
                        tirillaActual += String.Format("Codigo: {0}, Descripción: {1} ({4}), Cantidad: {2}, Precio: {3} ", x.Codigo, x.Descripcion, x.Cantidad, x.Subtotal, x.PrecioVentaUnidad);
                    });
                    log.Info("Items tirilla:" + tirillaActual + Environment.NewLine + "Total: " + Entorno.Instancia.Venta.TotalVenta + Environment.NewLine + "Totales Impuestos: " + Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2]) + Environment.NewLine + "Cant Artículos Vendidos: " + Entorno.Instancia.Venta.NumeroDeItemsVenta + Environment.NewLine);

                    var tiempoGuardarVenta = new MetricaTemporizador("TerminarVentaFinalizada");

                    venta.GuardarVenta(Entorno.Instancia.Venta, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.Venta).ToString(), factura, modeloImpresora, implementaImpuestoCompuesto, out respuesta);
                    if (respuesta.Valida == false)
                    {
                        Telemetria.Instancia.AgregaMetrica(tiempoGuardarVenta.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta));
                        throw new Exception(respuesta.Mensaje);
                    }

                    //Log a azure
                    Telemetria.Instancia.AgregaMetrica(tiempoGuardarVenta.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Venta.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Venta.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Venta.NumeroDeItemsVenta));

                    // 
                    Entorno.Instancia.Impresora.Imprimir(factura, true, true);


                    if (!string.IsNullOrEmpty(respuesta.Mensaje))
                    {
                        Entorno.Vista.PanelOperador.MensajeOperador = respuesta.Mensaje;
                        log.ErrorFormat("[CmdPagarVenta]: {0}", respuesta.Mensaje);
                    }

                    Entorno.Instancia.IdsAcumulados = idsAcumulados;

                    //
                    Entorno.Instancia.Venta.EstaAbierta = false;

                    //
                    respuesta = new Respuesta(false);
                    ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                    Entorno.Instancia.Terminal = terminal;

                    // 
                    LimpiarVentaFinalizada();

                    //
                    decimal valorCambio = Entorno.Instancia.Venta.PorPagar * -1;
                    iu.PanelVentas.VisorCliente.Total = 0;
                    iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0}", valorCambio.ToCustomCurrencyFormat());

                    try
                    {
                        decimal valorLimite = Entorno.Instancia.Parametros.ObtenerValorParametro<Decimal>("pdv.aviso_monto_max_en_caja");
                        decimal valorCaja = venta.DineroEnCaja(Entorno.Instancia.Terminal.Codigo, Entorno.Instancia.Usuario.IdUsuario, out respuesta);
                        log.InfoFormat("[CmdPagarVenta.Ejecutar] --> Valor en caja : {0}, Valor máximo: {1}", valorCaja.ToCustomCurrencyFormat(), valorLimite.ToCustomCurrencyFormat());
                        if (valorCaja > valorLimite)
                        {
                            iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0} - Tope máximo en caja excedido", valorCambio.ToCustomCurrencyFormat());
                        }
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat("[CmdPagarVenta] {0}", e.Message);
                        Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
                    }

                    log.InfoFormat("[CmdPagarVenta] --> Transaccion finalizada, cambio {0}", valorCambio.ToCustomCurrencyFormat());

                    // 
                    iu.PanelOperador.CodigoCliente = "";

                    iu.MostrarPanelVenta();
                }

                if (respuesta.Valida)
                {
                    iu.MostrarDisplayCliente(DisplayCliente.FinVenta);
                }
            }
            catch (Exception ex)
            {
                //
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
                log.ErrorFormat("[CmdPagarVenta] {0}", ex.Message);

                //
                iu.PanelOperador.MensajeOperador = ex.Message;
                EVenta copiaVenta = JsonConvert.DeserializeObject<EVenta>(jsonCopiaVenta);
                Entorno.Instancia.Venta = copiaVenta;
                Entorno.Instancia.Venta.ImpuestosIncluidos = copiaImpuestosIncluidos;

                //
                Solicitudes.SolicitudPagarVenta solVolver = new Solicitudes.SolicitudPagarVenta(Enums.Solicitud.Pagar, Entorno.Instancia.Vista.PanelPago.VisorEntrada);
                Reactor.Instancia.Procesar(solVolver);

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
