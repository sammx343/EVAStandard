using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Common;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EDegustacion : ETransaccion
    {
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
                IniciarDegustacion();
            }
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
            AgregarItemVenta(item);
            ActualizarImpuestosIncluidos(item, impuestos);
            return item;
        }
        public void IniciarDegustacion()
        {
            IniciarTransaccion();
        }
    }
}
