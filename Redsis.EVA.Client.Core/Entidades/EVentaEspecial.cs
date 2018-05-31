using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EVentaEspecial
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }

        /* Identifica si se deben discriminar cada uno de los impuestos,
        * true: si discrimina impuesto, false: no discrimina impuesto
        * Esto sólo aplica para el total de la venta (no por producto)
        * */
        #region Manejo de impuesto
        public bool ManejaImpto1 { get; set; }
        public bool ManejaImpto2 { get; set; }
        public bool ManejaImpto3 { get; set; }
        public bool ManejaImpto4 { get; set; }
        public bool ManejaImpto5 { get; set; }
        public bool ManejaImpto6 { get; set; } 
        #endregion

        /// <summary>
        /// leyenda de la factura
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Indica si la venta recibe pago o el valor total a pagar es cero.
        /// true: se tiene que permitir pagar el valor total de la factura,
        /// false: el valor a pagar es cero, finaliza la venta sin recibir medio de pago.
        /// </summary>
        public bool EsRqueridoPago { get; set; }

        /// <summary>
        /// Indica si es obligatorio asociar cliente a la venta
        /// </summary>
        public bool EsRequeridoCliente { get; set; }

        /// <summary>
        /// Indica si unicamente se agregan los clientes de la lista o no.
        /// true: solo se deben agregar los clientes de la lista asociados al tipo de venta, 
        /// false: no se limitan clientes para el tipo de venta definido.        
        /// </summary>
        public bool EsRequeridoClienteEspecifico { get; set; }

        /// <summary>
        /// Lista de todos los clientes asociados al tipo de venta definido.
        /// Si el valor de especificaClientes=true, se debe agregar por lo menos un cliente 
        /// en la lista para la definicion
        /// </summary>
        public EClientesVentaEspecial Clientes;

        /// <summary>
        /// Excepción general.
        /// </summary>
        public Exception ex { get; set; }

        public EVentaEspecial()
        {
            Id = "";
            Descripcion = "";
            ManejaImpto1 = false;
            ManejaImpto2 = false;
            ManejaImpto3 = false;
            ManejaImpto4 = false;
            Mensaje = "";
            EsRqueridoPago = false;
            EsRequeridoCliente = false;
            EsRequeridoClienteEspecifico = false;
        }

        public EVentaEspecial(string id, string desc, bool manejaImpuesto1, bool manejaImpuesto2, bool manejaImpuesto3, bool manejaImpuesto4, string mensaje, bool esRequeridoPago, bool esRequeridoCliente)
        {
            Id = id;
            Descripcion = desc;
            ManejaImpto1 = manejaImpuesto1;
            ManejaImpto2 = manejaImpuesto2;
            ManejaImpto3 = manejaImpuesto3;
            ManejaImpto4 = manejaImpuesto4;
            Mensaje = mensaje;
            EsRqueridoPago = esRequeridoPago;
            EsRequeridoCliente = esRequeridoCliente;
            EsRequeridoClienteEspecifico = false;
        }

        public EVentaEspecial(string id, string desc, bool manejaImpuesto1, bool manejaImpuesto2, bool manejaImpuesto3, bool manejaImpuesto4, string mensaje, bool esRequeridoPago, bool esRequeridoCliente, EClientesVentaEspecial clientes)
        {
            Id = id;
            Descripcion = desc;
            ManejaImpto1 = manejaImpuesto1;
            ManejaImpto2 = manejaImpuesto2;
            ManejaImpto3 = manejaImpuesto3;
            ManejaImpto4 = manejaImpuesto4;
            Mensaje = mensaje;
            EsRqueridoPago = esRequeridoPago;
            EsRequeridoCliente = esRequeridoCliente;
            EsRequeridoClienteEspecifico = true;
            Clientes = clientes;
        }

    }
}
