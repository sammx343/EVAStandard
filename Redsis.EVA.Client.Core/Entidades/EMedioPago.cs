using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EMedioPago : IEquatable<EMedioPago>
    {
        [JsonProperty]
        public string Codigo { get; private set; }
        [JsonProperty]
        public string Tipo { get; private set; }
        [JsonProperty]
        public string SubTipo { get; private set; }
        [JsonProperty]
        public bool SolicitaAutorizacion { get; private set; }
        [JsonProperty]
        public bool SolicitaBanco { get; private set; }
        [JsonProperty]
        public bool SolicitaDocumento { get; private set; }
        [JsonProperty]
        public bool SolicitaFranqueo { get; private set; }
        [JsonProperty]
        public bool SolicitaNumeroMeses { get; private set; }
        [JsonProperty]
        public bool PermiteCambio { get; private set; }
        [JsonProperty]
        public int ValorLimiteCambio { get; private set; }
        [JsonProperty]
        public decimal PorcentajeLimiteCambio { get; private set; }
        [JsonProperty]
        public string FormatoFranqueo { get; private set; }
        [JsonProperty]
        public bool VerificarPago { get; private set; }
        [JsonProperty]
        public bool AbreCajon { get; private set; }
        [JsonProperty]
        public bool Credito { get; private set; }

        public EMedioPago()
        {
            Codigo = "";
            Tipo = "";
            SubTipo = "";
            SolicitaAutorizacion = false;
            SolicitaBanco = false;
            SolicitaDocumento = false;
            SolicitaFranqueo = false;
            SolicitaNumeroMeses = false;
            PermiteCambio = false;
            ValorLimiteCambio = 0;
            PorcentajeLimiteCambio = 0;
            FormatoFranqueo = "";
            VerificarPago = false;
            AbreCajon = false;
            Credito = false;
        }

        public EMedioPago(string codigo, string tipo, string subtipo, byte solicitaAutorizacion, byte solicitaBanco, byte solicitaDocumento,
            byte solicitaFranqueo, byte solicitaNumeroMeses, byte permmiteCambio, int valorLimiteCambio, decimal porcentajeLimiteCambio,
            string formatoFranqueo, byte verificarPago, byte abreCajon, byte credito)
        {
            Codigo = codigo;
            Tipo = tipo;
            SubTipo = subtipo;
            SolicitaAutorizacion = Convert.ToBoolean(solicitaAutorizacion);
            SolicitaBanco = Convert.ToBoolean(solicitaBanco);
            SolicitaDocumento = Convert.ToBoolean(solicitaDocumento);
            SolicitaFranqueo = Convert.ToBoolean(solicitaFranqueo);
            SolicitaNumeroMeses = Convert.ToBoolean(solicitaNumeroMeses);
            PermiteCambio = Convert.ToBoolean(permmiteCambio);
            ValorLimiteCambio = valorLimiteCambio;
            PorcentajeLimiteCambio = porcentajeLimiteCambio;
            FormatoFranqueo = formatoFranqueo;
            VerificarPago = Convert.ToBoolean(verificarPago);
            AbreCajon = Convert.ToBoolean(abreCajon);
            Credito = Convert.ToBoolean(credito);
        }

        public override string ToString()
        {
            return string.Format(
                "Medio Pago [Codigo:{0}, Tipo {1}, Subtipo:{2}, SolicitaAutorizacion:{3}, SolicitaBanco:{4}, SolicitaDocumento:{5}, SolicitaFanqueo:{6}, SolicitaNumeroMeses:{7}, PermiteCambio:{8}, ValorLimiteCambio:{9}, PorcentajeLimiteCambio:{10}, FormatoFanqueo:{11}, VerificarPago:{12}, Credito:{13}]",
                Codigo,
                Tipo,
                SubTipo,
                SolicitaAutorizacion,
                SolicitaBanco,
                SolicitaDocumento,
                SolicitaFranqueo,
                SolicitaNumeroMeses,
                PermiteCambio,
                ValorLimiteCambio,
                PorcentajeLimiteCambio,
                FormatoFranqueo,
                VerificarPago,
                Credito
                );
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            EMedioPago objMedioPago = obj as EMedioPago;
            if (objMedioPago == null)
                return false;
            else
                return Equals(objMedioPago);
        }

        public override int GetHashCode()
        {
            return Codigo.GetHashCode();
        }

        bool IEquatable<EMedioPago>.Equals(EMedioPago item)
        {
            if (item == null)
                return false;
            return (this.Codigo.Equals(item.Codigo));
        }
    }
}
