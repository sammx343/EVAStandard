using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Helpers.Impresion
{

    public class Pie
    {
        public string NFactura { get; set; }
        public string MNFactura { get; set; }
        public string AtendidoPor { get; set; }
        public string MAtendidoPor { get; set; }
        public string FechaExp { get; set; }
        public string MFechaExp { get; set; }
        public string Rango { get; set; }
        public string MRango { get; set; }
        public string MensajeResolucion { get; set; }
        public string Resolucion { get; set; }
        public string FechaHora { get; set; }
        public string Usuario { get; set; }
        public string CodUsuario { get; set; }
        public string Mensaje { get; set; }
        public string MTotal { get; set; }
        public string Total { get; set; }
        public string MCambio { get; set; }
        public string Cambio { get; set; }

        public string Habilita { get; set; }
        public string TituloRegimen { get; set; }
        public string MensajeRegimen { get; set; }
        public string TituloBienvenidos { get; set; }
        public string MensajeBienvenidos { get; set; }
        public string Inscripcion1 { get; set; }
        public string Inscripcion2 { get; set; }
        public string PaginaWeb { get; set; }

        public bool ClienteActivo { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteId { get; set; }
        public string ClienteDir { get; set; }
        public string ClienteTel { get; set; }

        public string Encabezado { get; set; }
        public string Prefijo { get; set; }
        public string Cajero { get; set; }
        public string TRX { get; set; }
        public string Terminal { get; set; }
        public string TotDscto { get; set; }
        public bool DsctoGeneral { get; set; }
        public string Neto { get; set; }
        public string Porcentaje { get; set; }

        public List<DetalleTipoPago> listTPago { get; set; }

        public string MensajeTipoVenta { get; set; }
        public string Local { get; internal set; }
        public string DirLocal { get; internal set; }
    }

    public class DetalleTipoPago
    {
        public string nombre { get; set; }
        public string valor { get; set; }
    }
}
