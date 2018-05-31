using EvaPOS;
using Newtonsoft.Json;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using System;
using System.Linq;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdCancelarPago : ComandoAbstract
    {
        Solicitudes.SolicitudCancelarPago Solicitud;
        string ValorEntrada { get; set; }
        public IMedioPagoUI Pago { get; private set; }

        public CmdCancelarPago(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudCancelarPago;
            Pago = Solicitud.Pago;
        }

        public override void Ejecutar()
        {
            string jsonCopiaVenta = JsonConvert.SerializeObject(Entorno.Instancia.Venta, Formatting.Indented);
            var copiaImpuestosIncluidos = Entorno.Instancia.Venta.ImpuestosIncluidos;

            try
            {
                Respuesta respuesta = new Respuesta(false);
                Core.Solicitudes.SolicitudVolver volver = new Core.Solicitudes.SolicitudVolver(Core.Enums.Solicitud.Volver);

                if (Entorno.Instancia.Venta.Pagos.IsNullOrEmptyList())
                {
                    Reactor.Instancia.Procesar(volver);
                    return;
                }

                if (Entorno.Instancia.Venta.Pagos.Sum(x => x.Valor) == 0)
                {
                    Reactor.Instancia.Procesar(volver);
                    return;
                }

                if (Pago == null)
                    return;

                //
                decimal valorCancelado = Pago.ValorMedioPago;

                if (valorCancelado < 0)
                {
                    Entorno.Instancia.Vista.PanelPago.VisorMensaje = "No se puede cancelar el último pago";
                    //
                    Reactor.Instancia.Procesar(volver);
                    return;
                }
                else if (valorCancelado > 0)
                {
                    decimal valor = Entorno.Instancia.Venta.PorPagar + valorCancelado;
                    if (valor > Entorno.Instancia.Venta.TotalVenta)
                    {
                        Entorno.Instancia.Vista.PanelPago.VisorMensaje = "No se puede cancelar el pago seleccionado";
                        Reactor.Instancia.Procesar(volver);
                        return;
                    }
                }

                //
                EMedioPago medioPago = null;
                EPago ePago = null;

                if (Pago != null)
                {
                    medioPago = new PMediosPago().GetAllMediosPago().MedioPago(Pago.CodigoMedioPago);
                }
                else
                {
                    medioPago = new PMediosPago().GetAllMediosPago().MedioPago("1");
                }


                ePago = new EPago(medioPago, -valorCancelado);

                Entorno.Instancia.Venta.AgregarPago(medioPago, ePago, out respuesta);

                // agregar a lista de .medio de pago
                Entorno.Instancia.Vista.PanelPago.AgregarMedioPagoUI(new DTOs.DItemMedioPago { CodigoMedioPago = ePago.MedioPago.Codigo, NombreMedioPago = ePago.MedioPago.Tipo, ValorMedioPago = ePago.Valor });

                //
                Telemetria.Instancia.AgregaMetrica(new Evento("CancelarPago").AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("Valor", (ePago.Valor)));

                log.InfoFormat("[CmdCancelarPago] Pago Cancelado: {0}, Transacción: {1}, Factura {2}", ePago.Valor, (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));
                if (!respuesta.Valida)
                {
                    throw new Exception(respuesta.Mensaje);
                }
                //
                iu.PanelVentas.VisorCliente.Total = Entorno.Instancia.Venta.PorPagar;
                iu.PanelPago.VisorCliente.Total = Entorno.Instancia.Venta.PorPagar;
                iu.PanelPago.VisorEntrada = string.Empty;
                iu.PanelPago.VisorMensaje = "";

                //
                Reactor.Instancia.Procesar(volver);

                if (respuesta.Valida)
                {
                    iu.MostrarDisplayCliente(DisplayCliente.FinVenta);
                }
            }
            catch (Exception ex)
            {
                //
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
                log.ErrorFormat("[CmdCancelarPago] {0}", ex.Message);

                //
                iu.PanelOperador.MensajeOperador = ex.Message;
                EVenta copiaVenta = JsonConvert.DeserializeObject<EVenta>(jsonCopiaVenta);
                Entorno.Instancia.Venta = copiaVenta;
                Entorno.Instancia.Venta.ImpuestosIncluidos = copiaImpuestosIncluidos;
            }
        }
    }
}
