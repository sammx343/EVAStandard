using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EVentasEspecial
    {
        public List<EVentaEspecial> ListaVentas { get; private set; }

        public EVentasEspecial()
        {
            ListaVentas = new List<EVentaEspecial>();
        }

        public EVentaEspecial Venta(string id)
        {
            EVentaEspecial venta = ListaVentas.Find(x => String.Equals(id, x.Id, StringComparison.Ordinal));
            return venta;
        }
    }
}
