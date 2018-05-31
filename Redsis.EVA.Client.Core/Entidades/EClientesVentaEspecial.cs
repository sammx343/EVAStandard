using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EClientesVentaEspecial
    {
        #region Propediades
        public bool EsRequeridoTipoIdentificacion { get; set; }
        public bool EsRequeridoIdentificacion { get; set; }
        public bool EsRequeridoPrimerNombre { get; set; }
        public bool EsRequeridoSegundoNombre { get; set; }
        public bool EsRequeridoPrimerApellido { get; set; }
        public bool EsRequeridoSegundoApellido { get; set; }
        public bool EsRequeridoTipoCliente { get; set; }
        public bool EsRequeridoCiudad { get; set; }
        public bool EsRequeridoPais { get; set; }
        public bool EsRequeridoCelular { get; set; }
        public bool EsRequeridoTelefono { get; set; }
        public bool EsRequeridoCorreo { get; set; }
        public bool EsRequeridoDireccion { get; set; }
        public List<ECliente> ListClientes = new List<ECliente>(); 
        #endregion


        public EClientesVentaEspecial(bool esRequeridoTipoIdentificacion, bool esRequeridoIdentificacion, bool esRequeridoPrimerNombre, bool esRequeridoSegundoNombre, bool esRequeridoPrimerApellido, bool esRequeridoSegundoApellido, bool esRequeridoTipoCliente, bool esRequeridoCiudad, bool esRequeridoPais, bool esRequeridoCelular, bool esRequeridoTelefono, bool esRequeridoCorreo, bool esRequeridoDireccion, ECliente cliente)
        {
            EsRequeridoTipoIdentificacion = esRequeridoTipoIdentificacion;
            EsRequeridoIdentificacion = esRequeridoIdentificacion;
            EsRequeridoPrimerNombre = esRequeridoPrimerNombre;
            EsRequeridoSegundoNombre = esRequeridoSegundoNombre;
            EsRequeridoPrimerApellido = esRequeridoPrimerApellido;
            EsRequeridoSegundoApellido = esRequeridoSegundoApellido;
            EsRequeridoTipoCliente = esRequeridoTipoCliente;
            EsRequeridoCiudad = esRequeridoCiudad;
            EsRequeridoPais = esRequeridoPais;
            EsRequeridoCelular = esRequeridoCelular;
            EsRequeridoTelefono = esRequeridoTelefono;
            EsRequeridoCorreo = esRequeridoCorreo;
            EsRequeridoDireccion = esRequeridoDireccion;
            ListClientes.Add(cliente);
        }

        public EClientesVentaEspecial(bool esRequeridoTipoIdentificacion, bool esRequeridoIdentificacion, bool esRequeridoPrimerNombre, bool esRequeridoSegundoNombre, bool esRequeridoPrimerApellido, bool esRequeridoSegundoApellido, bool esRequeridoTipoCliente, bool esRequeridoCiudad, bool esRequeridoPais, bool esRequeridoCelular, bool esRequeridoTelefono, bool esRequeridoCorreo, bool esRequeridoDireccion)
        {
            EsRequeridoTipoIdentificacion = esRequeridoTipoIdentificacion;
            EsRequeridoIdentificacion = esRequeridoIdentificacion;
            EsRequeridoPrimerNombre = esRequeridoPrimerNombre;
            EsRequeridoSegundoNombre = esRequeridoSegundoNombre;
            EsRequeridoPrimerApellido = esRequeridoPrimerApellido;
            EsRequeridoSegundoApellido = esRequeridoSegundoApellido;
            EsRequeridoTipoCliente = esRequeridoTipoCliente;
            EsRequeridoCiudad = esRequeridoCiudad;
            EsRequeridoPais = esRequeridoPais;
            EsRequeridoCelular = esRequeridoCelular;
            EsRequeridoTelefono = esRequeridoTelefono;
            EsRequeridoCorreo = esRequeridoCorreo;
            EsRequeridoDireccion = esRequeridoDireccion;
        }
    }
}
