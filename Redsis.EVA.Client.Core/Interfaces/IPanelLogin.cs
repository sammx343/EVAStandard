using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelLogin : IPanelActivo
    {
        string IdUsuario { get; set; }
        string Clave { get; set; }
    }
}
