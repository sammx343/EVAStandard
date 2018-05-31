using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IVisorCliente
    {
        string Descripcion { get; set; }

        decimal ValorItem { get; set; }

        decimal Total { get; set; }

        int Items { get; set; }

        void LimpiarVisor();

        void LimpiarArticulo();
    }
}
