using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelPagoManual: IPanelActivo
    {
        string VisorOperador { get; set; }

        string VisorMensaje { get; set; }

        string VisorEntrada { get; set; }

        int Items { get; set; }

        decimal Total { get; set; }

        void LimpiarPagoFinalizado();

    }
}
