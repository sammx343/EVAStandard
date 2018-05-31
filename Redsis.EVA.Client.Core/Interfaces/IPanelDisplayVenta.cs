using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelDisplayVenta
    {
        string Descripcion { get; set; }
        decimal PrecioVenta { get; set; }
        decimal SubTotal { get; set; }
        decimal TotalVenta { get; set; }

        void MostrarItemDisplay(bool esConsultarPrecio = false);
    }
}
