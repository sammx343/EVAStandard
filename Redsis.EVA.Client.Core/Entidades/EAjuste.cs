using Redsis.EVA.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EAjuste : ETransaccion
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public String Estado { get; private set; } = "L";
        //public int Numero { get; private set; } = 0;
        //public decimal Total { get; private set; } = 0;
        //public ETipoAjuste Tipo { get; private set; }
        //protected List<EItemVenta> tirilla { get; private set; }  = new List<EItemVenta>();


        protected new bool EsValidoCancelarItem(EArticulo articulo, int cantidad, ref Respuesta respuesta)
        {
            bool esValido = true;

            int total = tirilla.Where(t => t.Articulo.Equals(articulo)).Sum(a => a.Cantidad);
            if ((Math.Abs(total) - Math.Abs(cantidad)) < 0)
            {
                log.Info("[ValidarCancelarItem]: Cantidad a cancelar mayor a la de la tirilla.");
                respuesta.Valida = false;
                respuesta.Mensaje = "La cantidad a cancelar no es valida";
                esValido = false;
            }
            return esValido;

        }

        //public EItemVenta AgregarArticulo(EArticulo articulo, int cantidad, string codigoLeido, List<EImpuesto> impuestos, out Respuesta respuesta)
        //{
        //    respuesta = new Respuesta(true);
        //    if (!EsValidoImpuesto(articulo, impuestos, ref respuesta))
        //        return null;

        //    if (!EsDiferenteCero(cantidad, ref respuesta))
        //        return null;

        //    if (cantidad < 0)
        //    {
        //        if (!EsValidoCancelarItem(articulo, cantidad, ref respuesta))
        //            return null;
        //    }

        //    decimal valor =articulo.PrecioVenta1 * cantidad;
        //    Total += valor;
        //    EItemVenta item = new EItemVenta(articulo, cantidad, valor, tirilla.Count + 1, CalcularImpuesto(valor, articulo.Impuesto1), codigoLeido);
        //    tirilla.Add(item);
        //    return item;
        //}

        public decimal TotalImpuesto()
        {
            decimal valor = 0;
            if (tirilla != null)
                valor = tirilla.Sum(x => x.Impuesto);
            return valor;
        }

        public override EItemVenta AgregarArticulo(EArticulo articulo, int cantidad, string codigoLeido, List<EImpuesto> impuestos, bool implementaImpuestoCompuesto, out Respuesta respuesta)
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

            if (!EsDiferenteCero(cantidad, ref respuesta))
                return null;

            if (cantidad < 0)
            {
                if (!EsValidoCancelarItem(articulo, cantidad, ref respuesta))
                    return null;
            }

            decimal valor = CalcularValor(articulo.PrecioVenta1, cantidad);
            if (cantidad > 0)
            {
                BrutoPositivo += valor;
            }
            else
            {
                BrutoNegativo += valor * -1;
            }
            TotalVenta = BrutoPositivo - BrutoNegativo;
            EItemVenta item = new EItemVenta(articulo, cantidad, valor, tirilla.Count + 1, CalcularImpuesto(valor, articulo.Impuesto1), codigoLeido);
            AgregarItemVenta(item);
            ActualizarImpuestosIncluidos(item, impuestos);
            return item;
        }
    }
}
