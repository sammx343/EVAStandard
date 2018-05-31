using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Enums
{
    public enum TipoTransaccion
    {
        Venta = 0,
        Arqueo = 6,
        Prestamo = 3,
        Recogida = 4,
        Inventario = 15,
        Cerrar = 14,
        Ajuste = 18,
        Devolucion = 20,
        AnularVenta = 7,
        VentaEspecial = 19,
        AbrirCajon = 21
    }
}
