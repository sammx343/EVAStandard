using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Common;

namespace Redsis.EVA.Client.Core.Entidades
{
    /// <summary>
    /// Clase Intervención
    /// </summary>
    public class EIntervencion
    {
        public string idVentasSupervisor { get; set; }
        public decimal version { get; set; }
        public string claveSupervisor { get; set; }
        public string codEmpresa { get; set; }
        public string codTerminal { get; set; }
        public string codUsuario { get; set; }
        public int consecutivoArt { get; set; }
        public int consecutivoPago { get; set; }
        public DateTime date_created { get; set; }
        public DateTime last_updated { get; set; }
        public string motivo { get; set; }
        public int nro_transac { get; set; }
        public string id_venta { get; set; }

        /// <summary>
        /// Indica si la transacción actual tiene o no intervención.
        /// </summary>
        public bool ExisteIntervencion { get; set; }

        public EIntervencion()
        {

        }

        public EIntervencion(string id_ventas_supervisor, string clave_sup, string motivo, string id_venta)
        {
            this.idVentasSupervisor = id_ventas_supervisor;
            this.claveSupervisor = clave_sup;
            this.motivo = motivo;
            this.id_venta = id_venta;
        }



        //public EIntervencion AgregarIntervencion()
        //{

        //}
    }
}
