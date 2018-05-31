using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IItemTirillaIU
    {
        string Codigo { get; set; }
        string Descripcion { get; set; }
        decimal PrecioVentaUnidad { get; set; }
        decimal Cantidad { get; set; }
        decimal Subtotal { get; set; }
    }
}
