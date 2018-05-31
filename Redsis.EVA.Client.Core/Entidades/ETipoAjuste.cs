using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ETipoAjuste
    {
        public string Id { get; private set; }
        public bool CalcularCostoVenta { get; private set; }
        public string Codigo { get; private set; }
        public string Descripcion { get; private set; }
        public bool EditarCostoVenta { get; private set; }
        public bool MostrarCostoVenta { get; private set; }
        public string Signo { get; private set; }
        public bool Activo { get; private set; }

        public ETipoAjuste()
        {
            Id = "";
            CalcularCostoVenta = false;
            Codigo = "";
            Descripcion = "";
            EditarCostoVenta = false;
            MostrarCostoVenta = false;
            Signo = "";
            Activo = false;
        }

        public ETipoAjuste(string id, byte calcularCostoVenta, string codigo, string descripcion, byte editarCostoVenta, byte mostrarCostoVenta, string signo, byte activo)
        {
            Id = id;
            CalcularCostoVenta = Convert.ToBoolean(calcularCostoVenta);
            Codigo = codigo;
            Descripcion = descripcion;
            EditarCostoVenta = Convert.ToBoolean(editarCostoVenta);
            MostrarCostoVenta = Convert.ToBoolean(mostrarCostoVenta);
            Signo = signo;
            Activo = Convert.ToBoolean(activo);
        }
    }
}
