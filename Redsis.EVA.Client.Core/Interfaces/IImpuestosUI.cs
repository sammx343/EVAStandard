using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IImpuestosUI
    {
        string Tipo { get; set; }
        decimal Compra { get; set; }
        decimal Base { get; set; }
        decimal Impuestos { get; set; }
    }
}
