using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Common;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EFacturaVentaEspecialSinMedioPago : ETransaccion
    {
        public EVentaEspecial TipoVentaEspecial { get; private set; }

        public EFacturaVentaEspecialSinMedioPago(EVentaEspecial tipoVenta)
        {
            TipoVentaEspecial = tipoVenta;
        }

        public override EItemVenta AgregarArticulo(EArticulo articulo, int cantidad, string codigoLeido, List<EImpuesto> impuestos, bool implementaImpuestoCompuesto, out Respuesta respuesta)
        {
            respuesta = new Respuesta(true);
            //Valida si la cantidad es diferente de cero.
            if (!EsDiferenteCero(cantidad, ref respuesta))
                return null;
            if (cantidad < 0)
            {
                if (!EsValidoCancelarItem(articulo, cantidad, ref respuesta))
                    return null;
            }
            if (!EstaAbierta)
            {
                IniciarVentaEspecial();
            }
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
            impuestoValidoVentaEspecial(articulo, impuestos);
            decimal valor = CalcularValor(articulo.PrecioVenta1, cantidad);
            EItemVenta item = new EItemVenta(articulo, cantidad, valor, tirilla.Count + 1, CalcularImpuesto(valor, articulo.Impuesto1), codigoLeido);
            if (cantidad > 0)
            {
                BrutoPositivo += valor;
            }
            else
            {
                BrutoNegativo += valor * -1;
            }
            TotalVenta = BrutoPositivo - BrutoNegativo;
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
            return item;
        }

        public void IniciarVentaEspecial()
        {
            IniciarTransaccion();
        }

        public new void AgregarCliente(ECliente cliente)
        {
            if (TipoVentaEspecial.EsRequeridoClienteEspecifico)
            {
                if(TipoVentaEspecial.Clientes.ListClientes != null)
                {
                    if (TipoVentaEspecial.Clientes.ListClientes.Exists(x => x == cliente))
                    {
                        Cliente = cliente;
                    }
                }
                
            }
        }
        private void impuestoValidoVentaEspecial(EArticulo articulo, List<EImpuesto> impuestos)
        {
            EImpuesto impuesto = impuestos.Find(x => x.Porcentaje == (float)articulo.Impuesto1);
            if ((!this.TipoVentaEspecial.ManejaImpto1 && impuesto.Orden == 1) || (!this.TipoVentaEspecial.ManejaImpto2 && impuesto.Orden == 2) || (!this.TipoVentaEspecial.ManejaImpto3 && impuesto.Orden == 3) || (!this.TipoVentaEspecial.ManejaImpto4 && impuesto.Orden == 4) || (!this.TipoVentaEspecial.ManejaImpto5 && impuesto.Orden == 5) || (!this.TipoVentaEspecial.ManejaImpto6 && impuesto.Orden == 6))
            {
                articulo.Impuesto1 = 0;
            }
            
        }

    }
}
