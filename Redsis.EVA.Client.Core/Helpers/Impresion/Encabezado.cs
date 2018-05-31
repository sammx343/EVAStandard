using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Helpers.Impresion
{
    public class Encabezado
    {
        public string Empresa { get; set; }

        public string DEmpresa { get; set; }

        public string Nit { get; set; }

        public string Direccion1 { get; set; }

        public string Direccion2 { get; set; }

        public string Ciudad { get; set; }

        public string Telefono { get; set; }

        public string TituloCambios { get; set; }

        public string MensajeCambios { get; set; }

        public string ClienteNombre { get; set; }

        public string ClienteId { get; set; }

        public string Tiquete { get; set; }

        public string Caja { get; set; }

        public string Fecha { get; set; }

        public string Hora { get; set; }

        public string Local { get; set; }

        public string DirLocal { get; set; }

        /// <summary>
        /// Descripción del Tipo de Venta Especial
        /// </summary>
        public string DescTipoVenta { get; set; }

        /// <summary>
        /// Especifica si se usa una impresora NCR 
        /// </summary>
        public bool impresoraNCR { get; set; } = false;
    }
}
