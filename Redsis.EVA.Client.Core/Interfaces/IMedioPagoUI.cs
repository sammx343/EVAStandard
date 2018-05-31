using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IMedioPagoUI
    {
        string CodigoMedioPago { get; set; }

        string NombreMedioPago { get; set; }

        decimal ValorMedioPago { get; set; }
    }
}
