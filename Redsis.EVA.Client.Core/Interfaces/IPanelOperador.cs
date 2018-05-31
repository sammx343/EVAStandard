using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelOperador
    {
        string NombreUsuario { get; set; }

        string CodigoTerminal { get; set; }

        string MensajeOperador { get; set; }

        string CodigoCliente { get; set; }
    }
}
