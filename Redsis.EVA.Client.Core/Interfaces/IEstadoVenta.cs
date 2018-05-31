using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IEstadoVenta
    {
        string Descripcion { get; set; }
        string IdUsuario { get; set; }

        void LimpiarVisor();
    }
}
