using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Helpers.Impresion
{
    public class DetalleFactura
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Valor { get; set; }
        public string tipoDscto { get; set; }
        public string ValorDscto { get; set; }
        public string BtipoDscto { get; set; }
        public bool BValorDscto { get; set; }
        public string Cantidad { get; set; }
        public string SubTotal { get; set; }
        public bool ValCantidad { get; set; }
        public bool PesoReq { get; set; }
        public string Peso { get; set; }
        public string SubTotalPeso { get; set; }
        public string Dscto { get; set; }
        public string Tdscto { get; set; }
        public string Impto { get; set; }
        public string PorcentajeImpto { get; set; }
        public string lineaDetalle56 { get; set; }
        public string lineaDetalleCant56 { get; set; }
    }
}
