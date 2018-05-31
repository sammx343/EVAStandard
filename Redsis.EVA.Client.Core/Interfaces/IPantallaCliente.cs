using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPantallaCliente
    {
        void MostraPantallaCliente();

        void MostrarVista(Enums.DisplayCliente tipoDisplayCliente);
    }
}
