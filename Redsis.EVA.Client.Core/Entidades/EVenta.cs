using Newtonsoft.Json;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EVenta : ETransaccion
    {
        #region Propiedades
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [JsonProperty]
        public List<EPago> Pagos = new List<EPago>();
        [JsonProperty]
        public decimal PorPagar = 0;
        #endregion

        #region Funciones y metodos

        /// <summary>
        /// El comando llama a agregar articulo y el agregar artículo verifica que la transaccion esté abierta, sino, la abre.
        /// todo: eliminar el parámetro impuestos, éste ya está en el entorno lo que puede causar que se pierda la referencia en algún momento de la ejecución.
        /// todo: globalizar el parámetro implementaImpuestoCompuesto para evitar valores errado al pasarlo como parámetro.
        /// </summary>
        /// <param name="articulo"></param>
        /// <param name="cantidad"></param>
        /// <param name="codigoLeido"></param>
        /// <param name="impuestos"></param>
        /// <param name="implementaImpuestoCompuesto"></param>
        /// <param name="respuesta"></param>
        /// <returns></returns>
        public override EItemVenta AgregarArticulo(EArticulo articulo, int cantidad, string codigoLeido, List<EImpuesto> impuestos, bool implementaImpuestoCompuesto, out Respuesta respuesta)
        {
            EItemVenta item = null;
            respuesta = new Respuesta();
            try
            {
                respuesta = new Respuesta(true);

                if (implementaImpuestoCompuesto)
                {
                    if (!EsValidoImpuestoCompuesto(articulo, ref respuesta))
                        return null;
                }
                else
                {
                    if (!EsValidoImpuesto(articulo, impuestos, ref respuesta))
                        return null;
                }


                //Valida si la cantidad es diferente de cero.
                if (!EsDiferenteCero(cantidad, ref respuesta))
                    return null;

                //Validar si es cancelacón
                if (cantidad < 0)
                {
                    if (!EsValidoCancelarItem(articulo, cantidad, ref respuesta))
                        return null;
                }

                if (!EstaAbierta)
                {
                    IniciarVenta();
                    PorPagar = 0;
                }

                decimal valor = CalcularValor(articulo.PrecioVenta1, cantidad);
                item = new EItemVenta(articulo, cantidad, valor, tirilla.Count + 1, CalcularImpuesto(valor, articulo.Impuesto1), codigoLeido);
                ActualizarTotales(cantidad, valor);
                PorPagar += valor;

                //Manejo nuevo de impuestos
                if (implementaImpuestoCompuesto)
                {
                    item.calcularImpuestos(cantidad); 
                }
                AgregarItemVenta(item);
                if (implementaImpuestoCompuesto)
                {
                    ActualizarImpuestosIncluidosCompuestos(item);
                }
                else
                {
                    ActualizarImpuestosIncluidos(item, impuestos);
                }
            }
            catch (Exception ex)
            {

            }
            return item;
        }

        public EPago AgregarPago(EMedioPago medioPago, EPago pago, out Respuesta respuesta)
        {
            respuesta = new Respuesta(false);
            if (VerificarPago(medioPago, pago.Valor, ref respuesta))
            {
                log.InfoFormat("[EVenta.Agregar] pago verificado [{0}]", medioPago.ToString());

                Pagos.Add(pago);
                PorPagar -= pago.Valor;
            }
            else
            {
                log.WarnFormat("[EVenta.Agregar] pago no verificado. [{0}]", medioPago.ToString());
            }

            return pago;
        }

        #endregion
        public void IniciarVenta()
        {
            Pagos.Clear();
            PorPagar = 0;
            IniciarTransaccion();

        }
        public decimal TotalPosibleCambio()
        {
            decimal total = 0;
            total = Pagos.Where(x => x.MedioPago.PermiteCambio).Sum(x => x.Valor);
            return total;
        }


        //Descontar cambio
        public void DescontarCambio()
        {
            decimal cambio = PorPagar * -1;
            if (cambio > 0)
            {
                EPago ultimo = Pagos.Last(pago => pago.MedioPago.PermiteCambio);
                ultimo.Valor -= cambio;
                Pagos.Last(pago => pago.MedioPago.PermiteCambio).Valor = ultimo.Valor;
            }
        }

        public void RevertirDescontarCambio()
        {
            decimal cambio = PorPagar * -1;
            EPago ultimo = Pagos.Last(pago => pago.MedioPago.PermiteCambio);
            ultimo.Valor += cambio;
            Pagos.Last(pago => pago.MedioPago.PermiteCambio).Valor = ultimo.Valor;
        }

        private bool VerificarPago(EMedioPago medioPago, decimal valor, ref Respuesta respuesta)
        {
            if (EstaAbierta)
            {
                if (TotalVenta != 0)
                {
                    //ToDo: Verificar valor minimo por medio de pago.
                    if (PorPagar >= valor || medioPago.PermiteCambio)
                    {
                        respuesta.Valida = true;
                        return true;
                    }
                    else
                    {
                        respuesta.Valida = false;
                        respuesta.Mensaje = "El medio de pago seleccionado no permite cambio, el monto debe ser menor o igual.";
                        log.Info("[VerificarPago]: El medio de pago seleccionado no permite cambio, el monto debe ser menor o igual.");
                        return false;
                    }
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "El total de la venta debe ser mayor a cero.";
                    log.Info("[VerificarPago]: El total de la venta debe ser mayor a cero.");
                    return false;
                }
            }
            else
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "Transaccion no iniciada.";
                log.Info("[VerificarPago]: Transaccion no iniciada.");
                return false;
            }
        }

    }
}
