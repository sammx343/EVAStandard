using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EItemVenta: IEquatable<EItemVenta>
    {
        [JsonProperty]
        public EArticulo Articulo { get; private set; }
        [JsonProperty]
        public string CodigoLeido { get; set; }
        [JsonProperty]
        public int Cantidad { get;  set; }
        [JsonProperty]
        public decimal Peso { get;  set; }
        [JsonProperty]
        public int Consecutivo { get; private set; }
        [JsonProperty]
        public decimal Impuesto { get; set; }
        [JsonProperty]
        public decimal Valor { get;  set; }

        /// <summary>
        /// Obtiene diccionario de impuestos asociados al artículo, TKey = detalle de la configuración del impuesto, TValue = valor calculado de los impuestos.
        /// </summary>
        [JsonProperty]
        public Dictionary<EImpuestosArticulo, decimal> Impuestos { get; private set; } = new Dictionary<EImpuestosArticulo, decimal>();

        public EItemVenta(EArticulo articulo, int cantidad, decimal valor, int consecutivo, decimal impuesto, string codigoLeido)
        {
            Articulo = articulo;
            CodigoLeido = codigoLeido;
            Peso = 0;
            Consecutivo = consecutivo;
            Impuesto = impuesto;
            Cantidad = cantidad;
            Valor = valor;
        }

        public override string ToString()
        {
            return string.Format(
                "EItemVenta [Codigo:{0}, Descripcion:{1}, ValorUnidad:{2}, Cantidad:{3}, Valor:{4}, Impuesto: {5}, Peso: {6}, PctjImpuesto: {7}]",
                Articulo.CodigoImpresion,
                Articulo.DescripcionLarga,
                Articulo.PrecioVenta1,
                Cantidad,
                Valor,
                Impuesto,
                Peso,
                Articulo.Impuesto1);
        }


        /// <summary>
        /// Calcula los impuestos asociados al artículo para su persistencia.
        /// llena la lista de impuestos [Impuestos]
        /// </summary>
        public void calcularImpuestos(int cantidad)
        {
            decimal valor = 0;
            double porcentaje = 0;
            decimal precioBase = 0;
            decimal signo = Valor > 0 ? 1 : -1;

            //Calcula el total que se debe calcular para cada impuesto, necesario para tener el precio base.
            foreach (EImpuestosArticulo impuesto in this.Articulo.Impuestos)
            {
                valor += impuesto.TipoImpuesto == 2 ? impuesto.Valor * signo * cantidad : 0;
                porcentaje += impuesto.TipoImpuesto == 1 ? impuesto.Porcentaje : 0;
            }

            precioBase = (Valor - valor) / (1 + ((decimal)porcentaje / 100));
            precioBase = Math.Round(precioBase, 0, MidpointRounding.AwayFromZero);

            //
            Impuestos = new Dictionary<EImpuestosArticulo, decimal>();
            decimal total = 0;
            
            //calcula el valor de cada impuesto, a partir del precio base.
            foreach (EImpuestosArticulo impuesto in this.Articulo.Impuestos)
            {
                decimal valorImpuesto = impuesto.TipoImpuesto == 2 ? impuesto.Valor * signo * cantidad : 0;
                valorImpuesto = impuesto.TipoImpuesto == 1 ? (precioBase * (decimal)impuesto.Porcentaje / 100) : valorImpuesto;
                valorImpuesto = Math.Round(valorImpuesto, 0, MidpointRounding.AwayFromZero);
                total += valorImpuesto;
                Impuestos.Add(impuesto, valorImpuesto);
            }

            this.Impuesto = total;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            EItemVenta objItemVenta = obj as EItemVenta;
            if (objItemVenta == null)
                return false;
            else
                return Equals(objItemVenta);
        }
        //La busqueda se hace sobre el Codigo de impresion del artículo.
        public override int GetHashCode()
        {
            return Articulo.CodigoImpresion.GetHashCode();
        }

        bool IEquatable<EItemVenta>.Equals(EItemVenta item)
        {
            if (item == null)
                return false;
            return (this.Articulo.CodigoImpresion.Equals(item.Articulo.CodigoImpresion));
        }

        public object Clone()
        {
            // TODO: Implementar Articulo.Clone() y modificar el codigo siguiente para usarlo.
            EItemVenta nuevoItem = new EItemVenta(
                Articulo,
                Cantidad,
                Valor,
                Consecutivo,
                Impuesto,
                CodigoLeido);
            nuevoItem.Peso = Peso;
            nuevoItem.CodigoLeido = CodigoLeido;
            return nuevoItem;
        }
    }
}
