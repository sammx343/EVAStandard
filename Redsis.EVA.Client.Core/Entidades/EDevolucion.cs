using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Enums;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EDevolucion : ETransaccion
    {
        public override EItemVenta AgregarArticulo(EArticulo articulo, int cantidad, string codigoLeido, List<EImpuesto> impuestos, bool implementaImpuestoCompuesto, out Respuesta respuesta)
        {
            respuesta = new Respuesta(true);
            //Valida si la cantidad es diferente de cero.
            if (!EsDiferenteCero(cantidad, ref respuesta))
                return null;
            if (cantidad > 0)
            {
                if (!EsValidoCancelarItem(articulo, cantidad, ref respuesta))
                    return null;
            }
            if (!EstaAbierta)
            {
                IniciarDevolucion();
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
            decimal valor = CalcularValor(articulo.PrecioVenta1, cantidad);
            EItemVenta item = new EItemVenta(articulo, cantidad, valor, tirilla.Count + 1, CalcularImpuesto(valor, articulo.Impuesto1), codigoLeido);
            ActualizarTotales(cantidad, valor);
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
        public void IniciarDevolucion()
        {
            IniciarTransaccion();
        }
    }
}
