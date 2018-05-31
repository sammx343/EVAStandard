using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EImpuestosArticulo
    {
        [JsonProperty]
        public int Id { get; private set; }
        [JsonProperty]
        public String Identificador { get; private set; }
        [JsonProperty]
        public String Descripcion { get; private set; }
        [JsonProperty]
        public double Porcentaje { get; private set; }
        [JsonProperty]
        public decimal Valor { get; private set; }

        /// <summary>
        /// Obtiene o establece el tipo de impuesto, indica si es por porcentaje (1) o valor (2).
        /// </summary>
        [JsonProperty]
        public int TipoImpuesto { get; private set; }

        public EImpuestosArticulo(int id, String identificador, String descripcion, double porcentaje, decimal valor, int tipoImpuesto)
        {
            Identificador = identificador;
            Descripcion = descripcion;
            Porcentaje = porcentaje;
            Valor = valor;
            Id = id;
            TipoImpuesto = tipoImpuesto;
        }
    }
}
