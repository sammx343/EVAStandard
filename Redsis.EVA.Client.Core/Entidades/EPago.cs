using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EPago
    {
        [JsonProperty]
        public EMedioPago MedioPago { get; private set; }
        [JsonProperty]
        public String CodBanco { get; set; } = "00";
        [JsonProperty]
        public decimal Valor { get; set; }
        [JsonProperty]
        public int NumeroDocumento { get; set; } = 0;
        [JsonProperty]
        public String NumeroCuenta { get; set; } = "00";
        [JsonProperty]
        public int MesesPlazo { get; set; } = 0;

        [JsonConstructor]
        public EPago()
        {

        }

        public EPago(EMedioPago medioPago, decimal valor)
        {
            MedioPago = medioPago;
            Valor = valor;
        }
        
        public EPago(EMedioPago medioPago, decimal valor, int numeroDocumento, String codBanco, String numeroCuenta, int mesesPlazo)
        {
            MedioPago = medioPago;
            CodBanco = codBanco;
            Valor = valor;
            NumeroDocumento = numeroDocumento;
            NumeroCuenta = numeroCuenta;
            MesesPlazo = mesesPlazo;
        }
    }

}
