using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ECopiaImpresion
    {
        public string NroTransac { get; set; }
        public string NroFactura { get; set; }
        public string Prefijo { get; set; }
        public string FechaTransaccion { get; set; }
        public string TipoTransac { get; set; }
        public string Usuario { get; set; }
        public string Terminal { get; set; }
        public string CodLocalidad { get; set; }
        public string Localidad { get; set; }
        public string Contenido { get; set; }

        public ECopiaImpresion()
        {
            NroTransac = "";
            NroFactura = "";
            Prefijo = "";
            FechaTransaccion = "";
            TipoTransac = "";
            Usuario = "";
            Terminal = "";
            CodLocalidad = "";
            Localidad = "";
            Contenido = "";

        }

        public ECopiaImpresion(string nroTransac, string nroFactura, string prefijo, string fechaTransaccion, string tipoTransac, string usuario, string terminal, string codLocalidad, string localidad, string contenido)
        {
            NroTransac = nroTransac;
            NroFactura = nroTransac;
            Prefijo = prefijo;
            FechaTransaccion = fechaTransaccion;
            TipoTransac = tipoTransac;
            Usuario = usuario;
            Terminal = terminal;
            CodLocalidad = codLocalidad;
            Localidad = localidad;
            Contenido = contenido;
        }
    }
}
