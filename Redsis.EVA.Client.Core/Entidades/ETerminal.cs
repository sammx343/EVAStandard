using Newtonsoft.Json;
using Redsis.EVA.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ETerminal : IEquatable<ETerminal>
    {
        [JsonProperty]
        public string Codigo { get; private set; }
        [JsonProperty]
        public string Prefijo { get; private set; }
        [JsonProperty]
        public long NumeroFacturasAviso { get; private set; }
        [JsonProperty]
        public string NumeroAutorizacionFacturacion { get; private set; }
        [JsonProperty]
        public long NumeroUltimaFactura { get; private set; }
        [JsonProperty]
        public long NumeroUltimaTransaccion { get; private set; }
        [JsonProperty]
        public DateTime FechaAutorizacionFacturacion { get; private set; }
        [JsonProperty]
        public int NumeroDiasAlarma { get; private set; }
        [JsonProperty]
        public long PrimeraFactura { get; private set; }
        [JsonProperty]
        public long FacturaFinal { get; private set; }
        [JsonProperty]
        public ELocalidad Localidad { get; private set; }

        public ETerminal()
        {
            Codigo = "";
            Prefijo = "";
            NumeroFacturasAviso = 0;
            NumeroAutorizacionFacturacion = "";
            NumeroUltimaFactura = 0;
            NumeroUltimaTransaccion = 0;
            FechaAutorizacionFacturacion = DateTime.Now;
            NumeroDiasAlarma = 0;
            Localidad = null;
        }

        public ETerminal(string codigo, string prefijo, long numeroFacturasAviso, string numeroAutorizacionFacturacion, long numeroUltimaFactura,
            long numeroUltimaTransaccion, DateTime fechaAutorizacionFacturacion, int numeroDiasAlarma,long primeraFactura, long facturaFinal, ELocalidad localidad)
        {
            Codigo = codigo;
            Prefijo = prefijo;
            NumeroFacturasAviso = numeroFacturasAviso;
            NumeroAutorizacionFacturacion = numeroAutorizacionFacturacion;
            NumeroUltimaFactura = numeroUltimaFactura;
            NumeroUltimaTransaccion = numeroUltimaTransaccion;
            FechaAutorizacionFacturacion = fechaAutorizacionFacturacion;
            NumeroDiasAlarma = numeroDiasAlarma;
            Localidad = localidad;
            PrimeraFactura =primeraFactura;
            FacturaFinal = facturaFinal;
    }

        public override string ToString()
        {
            return string.Format(
                "Terminal [Codigo:{0}, Prefijo {1}, NumeroFacturasAviso:{2}, NumeroAutorizacionFacturacion:{3}, NumeroUltimaFactura:{4}, NumeroUltimaTransaccion:{5}, FechaAutorizacionFacturacion:{6}, NumeroDiasAlarma{7}, DescripcionLocalidad{8}]",
                Codigo,
                Prefijo,
                NumeroFacturasAviso,
                NumeroAutorizacionFacturacion,
                NumeroUltimaFactura,
                NumeroUltimaTransaccion,
                FechaAutorizacionFacturacion,
                NumeroDiasAlarma
                );
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            ETerminal objTerminal = obj as ETerminal;
            if (objTerminal == null)
                return false;
            else
                return Equals(objTerminal);
        }

        public override int GetHashCode()
        {
            return Codigo.GetHashCode();
        }

        bool IEquatable<ETerminal>.Equals(ETerminal item)
        {
            if (item == null)
                return false;
            return (this.Codigo.Equals(item.Codigo));
        }

        public bool VerificarLimiteNumeracion(out Respuesta respuesta)
        {
            respuesta = new Respuesta(true);
            if (this.NumeroUltimaFactura >= this.FacturaFinal)
            {
                respuesta.Documentar(false, "No fue posible crear la factura porque la numeración está agotada.");
                return false;
            }else if(this.FacturaFinal - this.NumeroUltimaFactura < this.NumeroFacturasAviso)
            {
                respuesta.Documentar(true,"La numeración de factura esta próxima a agotarse.");
                return true;
            }
            else
            {
                respuesta.Valida=true;
                return true;
            }
        }

        public bool VerificarFechaAutorizacion(out Respuesta respuesta)
        {
            respuesta = new Respuesta(true);
            DateTime actual = DateTime.Today;
            DateTime limite = this.FechaAutorizacionFacturacion.AddYears(2);
            if (DateTime.Compare(actual,limite) > 0)
            {
                respuesta.Documentar(false, "No fue posible crear la factura porque la numeración está agotada.");
                return false;
            }
            else if ((limite - actual).Days <= this.NumeroDiasAlarma)
            {
                respuesta.Documentar(true, "Numeración de factura está llegando a su fecha de vencimiento.");
                return true;
            }
            else
            {
                respuesta.Valida = true;
                return true;
            }
        }

    }
}
