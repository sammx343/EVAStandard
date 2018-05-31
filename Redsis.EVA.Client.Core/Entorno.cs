using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.DTOs;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core
{
    public class Entorno
    {
        private static readonly Entorno instancia = new Entorno();

        public IVista Vista { get; set; }

        public EVenta Venta { get; set; }

        public EDevolucion Devolucion { get; set; }

        public EAjuste Ajuste { get; set; }

        public EFacturaVentaEspecialSinMedioPago VentaEspecialSinMedioPago { get; set; }

        public EPrestamo Prestamo { get; set; }

        public ERecogida Recogida { get; set; }

        //public ECaja Caja { get; set; }

        public ECodigosRecogida CodigosRecogida { get; set; }

        public EVentasEspecial TipoVentaEspecial { get; set; }

        public EParametros Parametros { get; set; }

        public EUsuario Usuario { get; set; }

        public EClientes Clientes { get; set; }

        public ETerminal Terminal { get; set; }

        public Dictionary<string, string> IdsAcumulados { get; set; }

        public EImpuestos Impuestos { get; set; }

        public List<DMedioPago> MediosPago { get; set; }

        public ETiposAjuste TiposAjustes { get; set; }

        public IImpresora Impresora { get; set; }

        public IBaseDato BaseDato { get; set; }

        private Dictionary<int, EError> MensajesError { get; set; }

        private Entorno()
        {
            Venta = new EVenta();
            MediosPago = new List<DMedioPago>();
            //Impresora = new DImpresora();
        }

        public static Entorno Instancia
        {
            get { return instancia; }
        }

        public void AddInterface(ref IVista iu)
        {
            Vista = iu;
        }

        public string getMensajeError(int cod)
        {
            return MensajesError[cod].Descripcion;
        }

        public void setMensajesError(Dictionary<int, EError> me)
        {
            MensajesError = me;
        }

    }
}
