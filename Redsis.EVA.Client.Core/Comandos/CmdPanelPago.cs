using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Redsis.EVA.Client.Common;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelPago : ComandoAbstract
    {
        Solicitudes.SolicitudPanelPago solicitudPanelPago;

        public CmdPanelPago(ISolicitud solicitud) : base(solicitud)
        {
            solicitudPanelPago = solicitud as Solicitudes.SolicitudPanelPago;
        }

        public override void Ejecutar()
        {
            if (!Entorno.Instancia.Venta.EstaAbierta)
            {
                iu.PanelVentas.VisorMensaje = "No existe transaccion activa.";
                Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
                Reactor.Instancia.Procesar(solicitudPanelVenta);
                return;
            }

            //
            decimal totalPosibleCambio = Entorno.Instancia.Venta.TotalPosibleCambio();

            //
            if (Entorno.Instancia.Venta.PorPagar == 0 && Entorno.Instancia.Venta.TotalVenta > 0)
            {
                //Guardar venta
                //No hay vuelto

                iu.MostrarPanelPagos();
                iu.MostrarDisplayCliente(DisplayCliente.FinVenta);


                iu.PanelPago.VisorMensaje = string.Empty;
                iu.PanelPago.VisorOperador = "Ingrese Valor";
                iu.PanelPago.VisorCliente.Total = iu.PanelVentas.VisorCliente.Total;
                iu.PanelPago.VisorCliente.Items = Entorno.Instancia.Venta.NumeroDeItemsVenta;

                ActualizarImpuestos();

                log.Info("[CmdPanelPago.Ejecutar.39] No hay saldo pendiente por pagar y no hay cambio pendiente, se registra la venta.");

                Entorno.Instancia.Venta.PorPagar = Entorno.Instancia.Venta.TotalVenta;
                Solicitudes.SolicitudPagarVenta solicitudPagarVenta = new Solicitudes.SolicitudPagarVenta(Enums.Solicitud.PagoEfectivo, "VentaPagada");
                Reactor.Instancia.Procesar(solicitudPagarVenta);

            }
            else if (Entorno.Instancia.Venta.PorPagar < 0 && totalPosibleCambio >= Entorno.Instancia.Venta.PorPagar * -1)
            {
                //Directamente guardar venta.
                //Dar vuelto venta.PorPagar*-1

                log.InfoFormat("[CmdPanelPago.Ejecutar.50] No hay saldo pendiente por pagar, se debe dar cambio [{0}] y se registra la venta.", totalPosibleCambio.ToCustomCurrencyFormat());

                //Se calcula el total ($) de artículos cancelados + el total de pagos realizados.
                decimal saldoCliente = Entorno.Instancia.Venta.BrutoNegativo + (Entorno.Instancia.Venta.Pagos.Sum(p => p.Valor));

                //se calcula el saldo que se debe dar al cliente restando al total de artículos que se han agregado a la venta (sin tener en cuenta que haya sido cancelado) - saldoCliente
                decimal saldoFavorCliente = Entorno.Instancia.Venta.BrutoPositivo - saldoCliente;

                if (Math.Abs(saldoFavorCliente) == Math.Abs(Entorno.Instancia.Venta.PorPagar))
                {
                    Solicitudes.SolicitudPagarVenta solicitudPagarVenta = new Solicitudes.SolicitudPagarVenta(Enums.Solicitud.PagoEfectivo, "VentaPagada");
                    Reactor.Instancia.Procesar(solicitudPagarVenta);
                }
            }
            else if (Entorno.Instancia.Venta.PorPagar < 0 && totalPosibleCambio < Entorno.Instancia.Venta.PorPagar * -1)
            {
                //No me permite pasar al panel de pago y no termina la transaccion
                //Solo queda por hacre agregar más artículos.

                log.InfoFormat("[CmdPanelPago.Ejecutar.59] No es posible entregar cambio, debe agregar más artículos."); ;
                iu.PanelVentas.VisorMensaje = "No es posible entregar cambio, debe agregar más artículos.";
            }
            else if (Entorno.Instancia.Venta.PorPagar > 0)
            {
                //Permitir agregar pagos.
                log.Info("[CmdPanelPago] Mostrando panel de pagos");

                iu.MostrarPanelPagos();
                iu.MostrarDisplayCliente(DisplayCliente.FinVenta);



                iu.PanelPago.VisorMensaje = (solicitudPanelPago == null) ? string.Empty : solicitudPanelPago?.VisorMensaje;
                if (iu.PanelOperador.MensajeOperador == iu.PanelPago.VisorMensaje)
                {
                    iu.PanelOperador.MensajeOperador = "";
                }
                else
                {
                    iu.PanelOperador.MensajeOperador = (string.IsNullOrEmpty(iu.PanelOperador.MensajeOperador) ? string.Empty : iu.PanelOperador.MensajeOperador);
                    if(Config.ViewMode == InternalSettings.ModoTouch)
                    {
                        iu.PanelOperador.MensajeOperador = iu.PanelPago.VisorMensaje;
                    }
                }

                iu.PanelPago.VisorOperador = "Ingrese Valor";
                iu.PanelPago.VisorCliente.Total = Entorno.Instancia.Venta.PorPagar;
                iu.PanelPago.VisorCliente.Items = Entorno.Instancia.Venta.NumeroDeItemsVenta;

                ActualizarImpuestos();
            }        
        }

        private void ActualizarImpuestos()
        {
            Dictionary<EImpuesto, List<decimal>> impuestosVenta = Entorno.Instancia.Venta.ImpuestosIncluidos;
            ObservableCollection<IImpuestosUI> listImpuestos = new ObservableCollection<IImpuestosUI>();

            foreach (var item in impuestosVenta)
            {
                DTOs.DImpuestos impuesto = new DTOs.DImpuestos();

                impuesto.Tipo = string.Format("{0}={1}%", item.Key.Identificador, item.Key.Porcentaje);
                impuesto.Compra = item.Value[0];
                impuesto.Base = item.Value[1];
                impuesto.Impuestos = item.Value[2];

                listImpuestos.Add(impuesto);
            }

            iu.PanelPago.AgregarImpuestosUI(listImpuestos);
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